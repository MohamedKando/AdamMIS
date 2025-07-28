
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
        public UserService(AppDbContext context, UserManager<ApplicationUser> userManager, IRoleService roleService)
        {
            _context = context;
            _userManager = userManager;
            _roleService = roleService;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            var users = await _context.Users.ToListAsync();
            return users;
        }
        public async Task<IEnumerable<UserResponse>> GetAllBannedUsersAsync()
        {
            var users = await (from u in _context.Users
                               join ur in _context.UserRoles
                               on u.Id equals ur.UserId
                               join r in _context.Roles
                               on ur.RoleId equals r.Id into roles
                               where (u.IsDisabled==true)
                               select new
                               {
                                   u.Id,
                                   u.UserName,
                                   u.IsDisabled,
                                   Roles = roles.Select(x => x.Name!).ToArray()


                               })
                     .GroupBy(u => new { u.Id, u.UserName, u.IsDisabled })
                     .Select(u => new UserResponse
                     {
                         Id = u.Key.Id,
                         UserName = u.Key.UserName!,
                         IsDisabled = u.Key.IsDisabled,
                         Roles = u.SelectMany(x => x.Roles)
                     })

                     .ToListAsync();

            return users;
        }

        public async Task<IEnumerable<UserResponse>> GetAllAsync()
        {

            var users = await (from u in _context.Users
                               join ur in _context.UserRoles
                               on u.Id equals ur.UserId
                               join r in _context.Roles
                               on ur.RoleId equals r.Id into roles
                               where (!roles.Any(x => x.Name == DeafultRole.SuperAdmin))
                               select new
                               {
                                   u.Id,
                                   u.UserName,
                                   u.IsDisabled,
                                   Roles = roles.Select(x => x.Name!).ToArray()


                               })
                     .GroupBy(u => new { u.Id, u.UserName, u.IsDisabled })
                     .Select(u => new UserResponse
                     {
                         Id = u.Key.Id,
                         UserName = u.Key.UserName!,
                         IsDisabled = u.Key.IsDisabled,
                         Roles = u.SelectMany(x => x.Roles)
                     })

                     .ToListAsync();

            return users;
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
    }
}
