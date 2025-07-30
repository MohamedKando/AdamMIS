
using AdamMIS.Contract.UserRole;
using AdamMIS.Contract.Users;
using AdamMIS.Errors;
using AdamMIS.Services.RolesServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query;

namespace AdamMIS.Services.UsersServices
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRoleService _roleService;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UserService(AppDbContext context, UserManager<ApplicationUser> userManager, IRoleService roleService, RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleService = roleService;
            _roleManager = roleManager;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            var users = await _context.Users.ToListAsync();
            return users;
        }
        public async Task<IEnumerable<UserResponse>> GetAllBannedUsersAsync()
        {
            var users = await (from u in _context.Users
                               join ur in _context.UserRoles on u.Id equals ur.UserId into userRoles
                               from ur in userRoles.DefaultIfEmpty()
                               join r in _context.Roles on ur.RoleId equals r.Id into roles
                               from r in roles.DefaultIfEmpty()

                               where u.IsDisabled==true

                               group new { u, r } by new { u.Id, u.UserName, u.IsDisabled } into g
                               select new UserResponse
                               {
                                   Id = g.Key.Id,
                                   UserName = g.Key.UserName!,
                                   IsDisabled = g.Key.IsDisabled,
                                   Roles = g.Where(x => x.r != null).Select(x => x.r!.Name!)
                               }).ToListAsync();


            return users;
        }

        public async Task<IEnumerable<UserResponse>> GetAllAsync()
        {
            var users = await (from u in _context.Users
                               join ur in _context.UserRoles on u.Id equals ur.UserId into userRoles
                               from ur in userRoles.DefaultIfEmpty()
                               join r in _context.Roles on ur.RoleId equals r.Id into roles
                               from r in roles.DefaultIfEmpty()

                               where r == null || r.Name != DeafultRole.SuperAdmin

                               group new { u, r } by new { u.Id, u.UserName, u.IsDisabled } into g
                               select new UserResponse
                               {
                                   Id = g.Key.Id,
                                   UserName = g.Key.UserName!,
                                   IsDisabled = g.Key.IsDisabled,
                                   Roles = g.Where(x => x.r != null).Select(x => x.r!.Name!)
                               }).ToListAsync();


            return users;
        }

        public async Task<Result<IEnumerable<string>>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result.Failure< IEnumerable<string>>(UserErrors.UserNotFound);

            var roles = await _userManager.GetRolesAsync(user);
            return Result.Success<IEnumerable<string>>(roles);
        }

        public async Task<Result<UserResponse>> AddUserAsync(CreateUserRequest request)
        {
            var userIsExist = await _userManager.Users.AnyAsync(x => x.UserName == request.UserName);
            if (userIsExist)
            {
                return Result.Failure<UserResponse>(UserErrors.DublicatedUser);

            }

            var allowedRoles = await _roleService.GetAllAsync();

            if (request.Roles.Except(allowedRoles.Select(x => x.Name)).Any())
                return Result.Failure<UserResponse>(RolesErrors.RoleNotFound);

            var user = request.Adapt<ApplicationUser>();
            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRolesAsync(user, request.Roles);
                var response = new UserResponse
                {
                    Id = user.Id,
                    UserName = user.UserName!,
                    IsDisabled = user.IsDisabled,
                    Roles = request.Roles
                };
                return Result.Success(response);
            }
            var error = result.Errors.First();
            return Result.Failure<UserResponse>(new Error(error.Code, error.Description));
        }


        // use this end point to ban user
        public async Task<Result> ToggleStatusAsync(string id)
        {

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                Result.Failure(UserErrors.UserNotFound);
            }

            user!.IsDisabled = !user.IsDisabled;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Result.Success();
            }
            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description));

        }













        public async Task<Result> UpdateUserRolesAsync(UserRoleRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return Result.Failure<UserRoleResponse>(UserErrors.UserNotFound);

            var currentRoles = await _userManager.GetRolesAsync(user);
            var allRoles = await _roleManager.Roles.ToListAsync();

            // Convert role IDs to names
            var requestedRoleNames = allRoles
                .Where(r => request.RoleIds.Contains(r.Id))
                .Select(r => r.Name)
                .ToList();

            // Determine what to add and what to remove
            var rolesToAdd = requestedRoleNames.Except(currentRoles).ToList();
            var rolesToRemove = currentRoles.Except(requestedRoleNames).ToList();

            if (rolesToRemove.Any())
                await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

            if (rolesToAdd.Any())
                await _userManager.AddToRolesAsync(user, rolesToAdd);

            var updatedRoles = await _userManager.GetRolesAsync(user);

            return Result.Success();
        }



    }
}
