using AdamMIS.Entities.SystemLogs;
using AdamMIS.Services.LogServices;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;

namespace AdamMIS.Abstractions.LoggingAbstractions
{
    public class AuditSaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditSaveChangesInterceptor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            AddAuditLogs(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            AddAuditLogs(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void AddAuditLogs(DbContext? context)
        {
            if (context == null) return;

            var username = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
            var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";

            var entries = context.ChangeTracker
                .Entries()
                .Where(e => e.Entity is not SystemLog && e.State != EntityState.Unchanged)
                .ToList();

            foreach (var entry in entries)
            {
                // Skip Modified entities that only have ignored property changes
                if (entry.State == EntityState.Modified && OnlyIgnoredPropertiesChanged(entry))
                    continue;

                var actionType = entry.State switch
                {
                    EntityState.Added => "Create",
                    EntityState.Modified => "Update",
                    EntityState.Deleted => "Delete",
                    _ => "Unknown"

                };
                // In your AddAuditLogs method, after determining actionType
                if (entry.State == EntityState.Modified)
                {
                    var significantChanges = GetSignificantChanges(entry);
                    if (!significantChanges.Any())
                        continue; // Skip if no significant changes
                }

                var log = new SystemLog
                {
                    Username = username,
                    ActionType = actionType,
                    EntityName = GetFriendlyEntityName(entry.Entity.GetType().Name),
                    EntityId = GetPrimaryKeyValue(entry),
                    Timestamp = DateTime.Now,
                    IpAddress = ipAddress,
                    Description = GenerateSmartDescription(entry, actionType),
                    OldValues = GetSmartOldValues(entry),
                    NewValues = GetSmartNewValues(entry)
                };

                context.Set<SystemLog>().Add(log);
            }
        }
        private bool OnlyIgnoredPropertiesChanged(EntityEntry entry)
        {
            var ignoredProperties = new[]
            {
        "ConcurrencyStamp", "SecurityStamp", "NormalizedUserName", "NormalizedEmail",
        "PasswordHash", "LockoutEnabled", "AccessFailedCount", "EmailConfirmed",
        "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnd"
    };

            var modifiedProperties = entry.Properties
                .Where(p => p.IsModified)
                .Select(p => p.Metadata.Name)
                .ToList();

            // If all modified properties are in the ignored list, skip logging
            return modifiedProperties.All(prop => ignoredProperties.Contains(prop));
        }
        private string GetFriendlyEntityName(string entityName)
        {
            return entityName switch
            {
                "ApplicationUser" => "User",
                "ApplicationRole" => "Role",
                "IdentityRoleClaim`1" => "Role Permission",
                "UserReports" => "Report Assignment",
                _ => entityName
            };
        }

        private string GenerateSmartDescription(EntityEntry entry, string actionType)
        {
            var entityName = GetFriendlyEntityName(entry.Entity.GetType().Name);

            return actionType switch
            {
                "Create" => GenerateCreateDescription(entry, entityName),
                "Update" => GenerateUpdateDescription(entry, entityName),
                "Delete" => GenerateDeleteDescription(entry, entityName),
                _ => $"Performed {actionType} on {entityName}"
            };
        }

        private string GenerateCreateDescription(EntityEntry entry, string entityName)
        {
            // Special handling for different entity types
            switch (entry.Entity.GetType().Name)
            {
                case "IdentityRoleClaim`1":
                    var claimValue = entry.Property("ClaimValue")?.CurrentValue?.ToString();
                    return $"Added permission '{claimValue}' to role";

                case "UserReports":
                    return "Assigned report to user";

                default:
                    return $"Created new {entityName.ToLower()}";
            }
        }

        private string GenerateUpdateDescription(EntityEntry entry, string entityName)
        {
            var changes = GetSignificantChanges(entry);

            if (changes.Any())
            {
                var changeText = string.Join(", ", changes.Take(3));
                var moreCount = Math.Max(0, changes.Count - 3);

                var description = $"Updated {entityName.ToLower()}: {changeText}";
                if (moreCount > 0)
                {
                    description += $", ... and {moreCount} more";
                }
                return description;
            }

            return $"Updated {entityName.ToLower()}";
        }

        private string GenerateDeleteDescription(EntityEntry entry, string entityName)
        {
            // Try to get a meaningful identifier
            var name = TryGetEntityName(entry);
            if (!string.IsNullOrEmpty(name))
            {
                return $"Deleted {entityName.ToLower()}: {name}";
            }

            return $"Deleted {entityName.ToLower()}";
        }

        private List<string> GetSignificantChanges(EntityEntry entry)
        {
            var changes = new List<string>();
            var ignoredProperties = new[]
            {
                "ConcurrencyStamp", "SecurityStamp", "NormalizedUserName", "NormalizedEmail",
                "PasswordHash", "LockoutEnabled", "AccessFailedCount", "EmailConfirmed",
                "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnd"
            };

            foreach (var property in entry.Properties)
            {
                if (ignoredProperties.Contains(property.Metadata.Name) || !property.IsModified)
                    continue;

                var oldValue = FormatValue(property.OriginalValue);
                var newValue = FormatValue(property.CurrentValue);

                if (oldValue != newValue)
                {
                    var fieldName = GetFriendlyFieldName(property.Metadata.Name);
                    changes.Add($"{fieldName}: '{oldValue}' → '{newValue}'");
                }
            }

            return changes;
        }

        private string GetFriendlyFieldName(string fieldName)
        {
            return fieldName switch
            {
                "IsDisabled" => "Status",
                "IsDeafult" => "Default Role",
                "ClaimType" => "Permission Type",
                "ClaimValue" => "Permission",
                "UserName" => "Username",
                "PhoneNumber" => "Phone",
                "InternalPhone" => "Extension",
                "DepartmentId" => "Department",
                "PhotoPath" => "Profile Photo",
                _ => System.Text.RegularExpressions.Regex.Replace(fieldName, "([a-z])([A-Z])", "$1 $2")
            };
        }

        private string FormatValue(object? value)
        {
            return value switch
            {
                null => "Not Set",
                bool b => b ? "Enabled" : "Disabled",
                string s when string.IsNullOrEmpty(s) => "Empty",
                string s => s,
                DateTime dt => dt.ToString("yyyy-MM-dd HH:mm"),
                _ => value.ToString() ?? "Unknown"
            };
        }

        private string TryGetEntityName(EntityEntry entry)
        {
            // Try common name properties
            var nameProperties = new[] { "Name", "Title", "UserName", "Email" };

            foreach (var propName in nameProperties)
            {
                try
                {
                    var nameProperty = entry.Property(propName);
                    var nameValue = nameProperty?.CurrentValue?.ToString();
                    if (!string.IsNullOrEmpty(nameValue))
                        return nameValue;
                }
                catch
                {
                    // Property doesn't exist, continue
                }
            }

            return string.Empty;
        }

        private string? GetSmartOldValues(EntityEntry entry)
        {
            if (entry.State == EntityState.Added)
                return null;

            if (entry.State == EntityState.Modified)
            {
                // Create a clean object with only changed properties
                var changedProperties = new Dictionary<string, object?>();
                var changesSummary = new Dictionary<string, string>();

                foreach (var property in entry.Properties.Where(p => p.IsModified))
                {
                    var propertyName = property.Metadata.Name;
                    var oldValue = property.OriginalValue;
                    var newValue = property.CurrentValue;

                    changedProperties[propertyName] = oldValue;
                    changesSummary[propertyName] = $"{FormatValue(oldValue)} → {FormatValue(newValue)}";
                }

                // Add changes summary for easier frontend processing
                changedProperties["_changesSummary"] = changesSummary;

                return JsonSerializer.Serialize(changedProperties, new JsonSerializerOptions
                {
                    WriteIndented = false,
                    PropertyNamingPolicy = null
                });
            }

            if (entry.State == EntityState.Deleted)
            {
                var originalValues = new Dictionary<string, object?>();
                foreach (var property in entry.Properties)
                {
                    originalValues[property.Metadata.Name] = property.OriginalValue;
                }

                return JsonSerializer.Serialize(originalValues, new JsonSerializerOptions
                {
                    WriteIndented = false,
                    PropertyNamingPolicy = null
                });
            }

            return null;
        }

        private string? GetSmartNewValues(EntityEntry entry)
        {
            if (entry.State == EntityState.Deleted)
                return null;

            if (entry.State == EntityState.Added)
            {
                // For new entities, only include meaningful properties
                var meaningfulProperties = new Dictionary<string, object?>();
                var ignoredProperties = new[]
                {
                    "ConcurrencyStamp", "SecurityStamp", "PasswordHash", "NormalizedUserName",
                    "NormalizedEmail", "LockoutEnabled", "AccessFailedCount", "EmailConfirmed",
                    "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnd"
                };

                foreach (var property in entry.Properties)
                {
                    if (ignoredProperties.Contains(property.Metadata.Name))
                        continue;

                    var value = property.CurrentValue;
                    if (value != null && !IsEmptyValue(value))
                    {
                        meaningfulProperties[property.Metadata.Name] = value;
                    }
                }

                return JsonSerializer.Serialize(meaningfulProperties, new JsonSerializerOptions
                {
                    WriteIndented = false,
                    PropertyNamingPolicy = null
                });
            }

            if (entry.State == EntityState.Modified)
            {
                // For updates, include changed properties with change summary
                var changedProperties = new Dictionary<string, object?>();
                var changesSummary = new Dictionary<string, string>();

                foreach (var property in entry.Properties.Where(p => p.IsModified))
                {
                    var propertyName = property.Metadata.Name;
                    var oldValue = property.OriginalValue;
                    var newValue = property.CurrentValue;

                    changedProperties[propertyName] = newValue;
                    changesSummary[propertyName] = $"{FormatValue(oldValue)} → {FormatValue(newValue)}";
                }

                // Add changes summary for easier frontend processing
                changedProperties["_changesSummary"] = changesSummary;

                return JsonSerializer.Serialize(changedProperties, new JsonSerializerOptions
                {
                    WriteIndented = false,
                    PropertyNamingPolicy = null
                });
            }

            return null;
        }

        private bool IsEmptyValue(object value)
        {
            return value switch
            {
                null => true,
                string s => string.IsNullOrEmpty(s),
                System.Collections.ICollection c => c.Count == 0,
                _ => false
            };
        }

        private string GetPrimaryKeyValue(EntityEntry entry)
        {
            var key = entry.Metadata.FindPrimaryKey();
            if (key == null) return "Unknown";

            var keyValues = key.Properties.Select(p => entry.Property(p.Name).CurrentValue?.ToString() ?? "null");
            return string.Join(",", keyValues);
        }
    }
}