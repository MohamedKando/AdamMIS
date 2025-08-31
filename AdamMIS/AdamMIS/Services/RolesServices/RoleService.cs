
using Newtonsoft.Json;

namespace AdamMIS.Services.RolesServices
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILoggingService _loggingService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RoleService(RoleManager<ApplicationRole> roleManager, AppDbContext context,
            UserManager<ApplicationUser> userManager, ILoggingService loggingService,
            IHttpContextAccessor httpContextAccessor)
        {
            _roleManager = roleManager;
            _context = context;
            _userManager = userManager;
            _loggingService = loggingService;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetCurrentUsername()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
        }

        public async Task<IEnumerable<RolesResponse>> GetAllAsync(bool? includeDisabled = false)
        {
            var roles = await _roleManager.Roles
                .Where(x => x.Name != "SuperAdmin" && (!x.IsDeleted || (includeDisabled == true)))
                .ProjectToType<RolesResponse>()
                .ToListAsync();

            return roles;
        }

        public async Task<RolesDetailsReponse> GetRolesDetailsAsync(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return null;
            }

            var permissions = await _roleManager.GetClaimsAsync(role);

            var response = new RolesDetailsReponse()
            {
                Id = roleId,
                IsDeleted = role.IsDeleted,
                Name = role.Name!,
                Permissions = permissions.Select(x => x.Value)
            };

            return response;
        }

        public async Task<Result<RolesDetailsReponse>> AddRoleAsync(RoleRequest request)
        {
            var isExist = await _roleManager.RoleExistsAsync(request.Name);
            if (isExist)
            {
                return Result.Failure<RolesDetailsReponse>(RolesErrors.DuplicatedRole);
            }

            var allowedPermission = Permissions.GetAllPermissions();
            if (request.Permissions.Except(allowedPermission).Any())
            {
                return Result.Failure<RolesDetailsReponse>(RolesErrors.PermissionNotFound);
            }

            var role = new ApplicationRole
            {
                Name = request.Name,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                IsDeafult = false
            };

            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                var permissions = request.Permissions.Select(x => new IdentityRoleClaim<string>
                {
                    ClaimType = Permissions.Type,
                    ClaimValue = x,
                    RoleId = role.Id
                });

                await _context.AddRangeAsync(permissions);
                await _context.SaveChangesAsync();

                // Manual logging for role creation
                await _loggingService.LogAsync(new CreateLogRequest
                {
                    Username = GetCurrentUsername(),
                    ActionType = "Create",
                    EntityName = "Role",
                    EntityId = role.Id,
                    Description = $"Created new role '{request.Name}' with {request.Permissions.Count()} permissions",
                    OldValues = null,
                    NewValues = JsonConvert.SerializeObject(new
                    {
                        RoleId = role.Id,
                        RoleName = request.Name,
                        IsDefault = false,
                        PermissionsCount = request.Permissions.Count(),
                        Permissions = request.Permissions.ToList(),
                        CreatedAt = DateTime.UtcNow
                    })
                });

                var response = new RolesDetailsReponse
                {
                    Id = role.Id,
                    Name = request.Name,
                    Permissions = request.Permissions
                };

                return Result.Success<RolesDetailsReponse>(response);
            }

            // Use error from result for failure case
            var error = result.Errors.First();
            return Result.Failure<RolesDetailsReponse>(new Error(error.Code, error.Description, 0));
        }

        public async Task<bool> UpdateRoleAsync(string id, RoleRequest request)
        {
            var isExist = await _roleManager.Roles.AllAsync(x => x.Name == request.Name && x.Id != id);
            if (isExist)
            {
                return false;
            }

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return false;
            }

            var allowedPermission = Permissions.GetAllPermissions();
            if (request.Permissions.Except(allowedPermission).Any())
            {
                return false;
            }

            // Store old values for logging
            var currentPermissions = await _context.RoleClaims
                .Where(x => x.RoleId == id && x.ClaimType == Permissions.Type)
                .Select(x => x.ClaimValue)
                .ToListAsync();

            var oldValues = new
            {
                RoleName = role.Name,
                PermissionsCount = currentPermissions.Count,
                Permissions = currentPermissions
            };

            role.Name = request.Name;
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                var newPermissions = request.Permissions.Except(currentPermissions).Select(x => new IdentityRoleClaim<string>
                {
                    ClaimType = Permissions.Type,
                    ClaimValue = x,
                    RoleId = role.Id
                });

                var removedPermissions = currentPermissions.Except(request.Permissions);

                await _context.RoleClaims.Where(x => x.RoleId == id && removedPermissions.Contains(x.ClaimValue)).ExecuteDeleteAsync();
                await _context.AddRangeAsync(newPermissions);
                await _context.SaveChangesAsync();

                // Manual logging for role update
                await _loggingService.LogAsync(new CreateLogRequest
                {
                    Username = GetCurrentUsername(),
                    ActionType = "Update",
                    EntityName = "Role",
                    EntityId = id,
                    Description = $"Updated role '{request.Name}' - Modified permissions",
                    OldValues = JsonConvert.SerializeObject(oldValues),
                    NewValues = JsonConvert.SerializeObject(new
                    {
                        RoleName = request.Name,
                        PermissionsCount = request.Permissions.Count(),
                        Permissions = request.Permissions.ToList(),
                        AddedPermissions = newPermissions.Select(x => x.ClaimValue).ToList(),
                        RemovedPermissions = removedPermissions.ToList(),
                        UpdatedAt = DateTime.UtcNow
                    })
                });

                return true;
            }

            return false;
        }

        public async Task<bool> ToggleStatusAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return false;

            // Store old values for logging
            var oldStatus = role.IsDeleted;
            var usersWithRole = await _userManager.GetUsersInRoleAsync(role.Name);

            var oldValues = new
            {
                RoleName = role.Name,
                IsDeleted = oldStatus,
                UsersCount = usersWithRole.Count,
                UserNames = usersWithRole.Select(u => u.UserName).ToList()
            };

            role.IsDeleted = !role.IsDeleted;

            var updateResult = await _roleManager.UpdateAsync(role);
            if (!updateResult.Succeeded)
                return false;

            // If disabling the role, remove it from all users
            if (role.IsDeleted)
            {
                foreach (var user in usersWithRole)
                {
                    await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
            }

            // Manual logging for role status toggle
            await _loggingService.LogAsync(new CreateLogRequest
            {
                Username = GetCurrentUsername(),
                ActionType = role.IsDeleted ? "Disable" : "Enable",
                EntityName = "Role",
                EntityId = id,
                Description = $"{(role.IsDeleted ? "Disabled" : "Enabled")} role '{role.Name}'" +
                             (role.IsDeleted && usersWithRole.Any() ? $" and removed from {usersWithRole.Count} users" : ""),
                OldValues = JsonConvert.SerializeObject(oldValues),
                NewValues = JsonConvert.SerializeObject(new
                {
                    RoleName = role.Name,
                    IsDeleted = role.IsDeleted,
                    UsersRemovedCount = role.IsDeleted ? usersWithRole.Count : 0,
                    UpdatedAt = DateTime.UtcNow
                })
            });

            return true;
        }
    }
}