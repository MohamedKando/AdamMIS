using AdamMIS.Entities.UserEntities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AdamMIS.Authentications.Filters
{
    public class PermissionsSeeder
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public PermissionsSeeder(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task SeedPermissionsAsync()
        {
            // 1. Get all current permissions from the class
            var codePermissions = Permissions.GetAllPermissions();

            // 2. Get all permissions from the DB
            var superAdminRole = await _roleManager.FindByNameAsync("SuperAdmin");
            if (superAdminRole == null) return;

            var roleClaims = await _roleManager.GetClaimsAsync(superAdminRole);
            var dbPermissions = roleClaims
                .Where(c => c.Type == Permissions.Type)
                .Select(c => c.Value)
                .ToList();

            // 3. Add new permissions (in class but not in DB)
            var newPermissions = codePermissions.Except(dbPermissions).ToList();
            foreach (var perm in newPermissions)
            {
                await _roleManager.AddClaimAsync(superAdminRole, new Claim(Permissions.Type, perm!));
            }

            // 4. Remove deleted permissions (in DB but not in class)
            var removedPermissions = dbPermissions.Except(codePermissions).ToList();
            foreach (var perm in removedPermissions)
            {
                await _roleManager.RemoveClaimAsync(superAdminRole, new Claim(Permissions.Type, perm!));
            }
        }
    }
}
