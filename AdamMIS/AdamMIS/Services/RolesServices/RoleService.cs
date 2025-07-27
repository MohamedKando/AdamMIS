using AdamMIS.Abstractions.Consts;
using AdamMIS.Contract.Roles;
using AdamMIS.Errors;
using Microsoft.AspNetCore.Identity;

namespace AdamMIS.Services.RolesServices
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly AppDbContext _context;
        public RoleService(RoleManager<ApplicationRole> roleManager , AppDbContext context)
        {
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IEnumerable<RolesResponse>> GetAllAsync(bool? includeDisabled = false)
        {
           var roles= await _roleManager.Roles.Where(x => !x.IsDeafult && (!x.IsDeleted||(includeDisabled==true))).ProjectToType<RolesResponse>().ToListAsync();

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
                Id= roleId,
                IsDeleted=role.IsDeleted,
                Name=role.Name!,
                Permissions=permissions.Select(x => x.Value)
            };

            return response;

        }


        public async Task<Result<RolesDetailsReponse>> AddRoleAsync (RoleRequest request)
        {

            var isExsit = await _roleManager.RoleExistsAsync(request.Name);
            if (isExsit)
            {
                return Result.Failure<RolesDetailsReponse>(RolesErrors.DublicatedRole);
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
                IsDeafult=false
            };
           var result = await _roleManager.CreateAsync(role);
            if(result.Succeeded)
            {

                var permissions = request.Permissions.Select(x => new IdentityRoleClaim<string>
                {
                    ClaimType = Permissions.Type,
                    ClaimValue = x,
                    RoleId = role.Id
                });
                await _context.AddRangeAsync(permissions);
                await _context.SaveChangesAsync();

                var response = new RolesDetailsReponse
                {
                    Id = role.Id,
                    Name = request.Name,
                    Permissions = request.Permissions


                };
                return Result.Success<RolesDetailsReponse>(response);
            }

            // ast5dm el error dh lma t3ml el result pattern 
            var error = result.Errors.First();
            return Result.Failure<RolesDetailsReponse>(new Error(error.Code, error.Description));
        }

        public async Task<bool> UpdateRoleAsync(string id ,RoleRequest request)
        {

            var isExsit = await _roleManager.Roles.AllAsync(x => x.Name == request.Name && x.Id != id);
            if (isExsit)
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

            role.Name = request.Name;
            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                var currentPermissions =await _context.RoleClaims.Where(x => x.RoleId == id && x.ClaimType == Permissions.Type).Select(x => x.ClaimValue).ToListAsync();
                var newPermissions = request.Permissions.Except(currentPermissions).Select(x => new IdentityRoleClaim<string>
                {
                    ClaimType = Permissions.Type,
                    ClaimValue = x,
                    RoleId = role.Id
                });

                var removedPermissions = currentPermissions.Except(request.Permissions);

                await _context.RoleClaims.Where(x=> x.RoleId == id && removedPermissions.Contains(x.ClaimValue)).ExecuteDeleteAsync();

                await _context.AddRangeAsync(newPermissions);
                await _context.SaveChangesAsync();

                return true;
            }

            // ast5dm el error dh lma t3ml el result pattern 
            //var error = result.Errors.First();
            return false;
        }



        public async Task <bool> ToggleStatusAsync (string id)
        {
            var role = await _roleManager.FindByIdAsync (id);
            if (role == null)
            {
                return false;
            }

            role.IsDeleted =  !role.IsDeleted;


            await _roleManager.UpdateAsync(role);
            return true;

        }
    }
}
