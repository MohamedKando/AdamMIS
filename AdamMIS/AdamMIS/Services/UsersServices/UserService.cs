
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













        public async Task<Result<IEnumerable<UserRoleResponse>>> AssignRolesToUserAsync(UserRoleRequest request, string assignedBy)
        {
            var assignments = new List<ApplicationUserRole>();
            var identityResult = new IdentityResult { };
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return Result.Failure<IEnumerable<UserRoleResponse>>(RolesErrors.RoleNotFound);

            var assignedByUser = await _userManager.FindByNameAsync(assignedBy);
            if (assignedByUser == null)
                return Result.Failure<IEnumerable<UserRoleResponse>>(UserErrors.UserNotFound);

            var successfulAssignments = new List<string>();

            foreach (var roleId in request.RoleIds)
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null || role.IsDeleted)
                    continue;

                var existingAssignment = await _context.ApplicationUserRole
                    .FirstOrDefaultAsync(ur => ur.UserId == request.UserId &&
                                               ur.RoleId == roleId);


                if (existingAssignment == null)
                {
                    identityResult = await _userManager.AddToRoleAsync(user, role.Name);
                    if (identityResult.Succeeded)
                    {
                        var assignment = new ApplicationUserRole
                        {
                            UserId = request.UserId,
                            RoleId = roleId,

                            RoleName = role.Name,
                            AssignedBy = assignedBy,
                            AssignedAt = DateTime.UtcNow,
                            IsActive = true
                        };

                        assignments.Add(assignment);
                        successfulAssignments.Add(role.Name);
                    }
                }
                else if (existingAssignment != null)
                {
                    return Result.Failure<IEnumerable<UserRoleResponse>>(RolesErrors.UserRoleExist);
                }
            }

            if (successfulAssignments.Any())
            {
                await _context.ApplicationUserRole.AddRangeAsync(assignments);
                await _context.SaveChangesAsync();


                var response = assignments.Adapt<List<UserRoleResponse>>();
                return Result.Success<IEnumerable<UserRoleResponse>>(response);
            }


            var error = identityResult.Errors.First();

            return Result.Failure<IEnumerable<UserRoleResponse>>(new Error(error.Code, error.Description));


        }

        public async Task<Result> RemoveRoleFromUserAsync(UserRoleRequest request)
        {
            var identityResult = new IdentityResult { };
            var successfulRemoves = new List<string>();
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return Result.Failure(UserErrors.UserNotFound);
            foreach (var roleId in request.RoleIds)
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null)
                    return Result.Failure(RolesErrors.RoleNotFound);

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    // Remove from Identity system
                    identityResult = await _userManager.RemoveFromRoleAsync(user, role.Name);
                    if (identityResult.Succeeded)
                    {


                        successfulRemoves.Add(roleId);

                        
                    }

                    

                }
            }

            if(successfulRemoves.Any())
            {
                return Result.Success();
            }
            var error = identityResult.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description));
        }


    }
}
