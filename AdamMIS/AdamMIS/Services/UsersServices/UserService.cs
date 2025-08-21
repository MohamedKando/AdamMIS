using AdamMIS.Contract.Departments;
using AdamMIS.Contract.UserRole;
using AdamMIS.Contract.Users;
using AdamMIS.Contract.SystemLogs;
using AdamMIS.Errors;
using AdamMIS.Services.RolesServices;
using AdamMIS.Services.LogServices;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query;
using Newtonsoft.Json;
using System.Collections.Generic;
using AdamMIS.Entities.UserEntities;

namespace AdamMIS.Services.UsersServices
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRoleService _roleService;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IWebHostEnvironment _env;
        private readonly ILoggingService _loggingService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(AppDbContext context, UserManager<ApplicationUser> userManager,
            IRoleService roleService, RoleManager<ApplicationRole> roleManager,
            IWebHostEnvironment env, ILoggingService loggingService,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _roleService = roleService;
            _roleManager = roleManager;
            _env = env;
            _loggingService = loggingService;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetCurrentUsername()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
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

                               where u.IsDisabled == true

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
            var users = await (
                from u in _context.Users
                join d in _context.Departments on u.DepartmentId equals d.Id into departments
                from d in departments.DefaultIfEmpty()

                join ur in _context.UserRoles on u.Id equals ur.UserId into userRoles
                from ur in userRoles.DefaultIfEmpty()
                join r in _context.Roles on ur.RoleId equals r.Id into roles
                from r in roles.DefaultIfEmpty()

                where r == null || r.Name != DeafultRole.SuperAdmin

                group new { u, r, d } by new { u.Id, u.UserName, u.IsDisabled, u.Title, DepartmentName = d.Name } into g

                select new UserResponse
                {
                    Id = g.Key.Id,
                    UserName = g.Key.UserName!,
                    IsDisabled = g.Key.IsDisabled,
                    Title = g.Key.Title,
                    DepartmentName = g.Key.DepartmentName,
                    Roles = g.Where(x => x.r != null).Select(x => x.r!.Name!)
                    // Remove IndividualPermissions from here - we'll load them separately
                }
            ).ToListAsync();

            // Load individual permissions separately to avoid EF Core translation issues
            var userIds = users.Select(u => u.Id).ToList();
            var individualPermissions = await _context.UserPermissions
                .Where(up => userIds.Contains(up.UserId))
                .Select(up => new { up.UserId, up.Permission })
                .ToListAsync();

            // Group permissions by user ID
            var permissionsByUser = individualPermissions
                .GroupBy(p => p.UserId)
                .ToDictionary(g => g.Key, g => g.Select(p => p.Permission).ToList());

            // Assign individual permissions to each user
            foreach (var user in users)
            {
                user.IndividualPermissions = permissionsByUser.GetValueOrDefault(user.Id, new List<string>());
            }

            return users;
        }

        public async Task<Result<IEnumerable<string>>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result.Failure<IEnumerable<string>>(UserErrors.UserNotFound);

            var roles = await _userManager.GetRolesAsync(user);
            return Result.Success<IEnumerable<string>>(roles);
        }

        public async Task<Result<UserResponse>> AddUserAsync(CreateUserRequest request)
        {
            var userIsExist = await _userManager.Users.AnyAsync(x => x.UserName == request.UserName);
            if (userIsExist)
                return Result.Failure<UserResponse>(UserErrors.DublicatedUser);

            var allowedDepartments = await _context.Departments.ToListAsync();
            var department = allowedDepartments.FirstOrDefault(x => x.Name == request.DepartmentName);
            if (department == null)
                return Result.Failure<UserResponse>(DepartmentErrors.DepartmentNotFound);

            var allowedRoles = await _roleService.GetAllAsync();
            if (request.Roles.Except(allowedRoles.Select(x => x.Name)).Any())
                return Result.Failure<UserResponse>(RolesErrors.RoleNotFound);

            var user = request.Adapt<ApplicationUser>();
            user.DepartmentId = department.Id;
            user.Title = request.Title;

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                return Result.Failure<UserResponse>(new Error(error.Code, error.Description, 0));
            }

            await _userManager.AddToRolesAsync(user, request.Roles);

            // Manual logging for user creation
            await _loggingService.LogAsync(new CreateLogRequest
            {
                Username = GetCurrentUsername(),
                ActionType = "Create",
                EntityName = "User",
                EntityId = user.Id,
                Description = $"Created new user '{user.UserName}' with {request.Roles.Count()} roles",
                OldValues = null,
                NewValues = JsonConvert.SerializeObject(new
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Title = user.Title,
                    DepartmentName = department.Name,
                    RolesCount = request.Roles.Count(),
                    Roles = request.Roles.ToList(),
                    IsDisabled = user.IsDisabled,
                    CreatedAt = DateTime.UtcNow
                })
            });

            var response = new UserResponse
            {
                Id = user.Id,
                UserName = user.UserName!,
                IsDisabled = user.IsDisabled,
                Roles = request.Roles,
                DepartmentName = department.Name,
                Title = request.Title,
            };

            return Result.Success(response);
        }

        public async Task<Result> ToggleStatusAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return Result.Failure(UserErrors.UserNotFound);
            }

            // Store old values for logging
            var oldStatus = user.IsDisabled;
            var userRoles = await _userManager.GetRolesAsync(user);

            var oldValues = new
            {
                UserName = user.UserName,
                IsDisabled = oldStatus,
                Title = user.Title,
                Roles = userRoles.ToList()
            };

            user.IsDisabled = !user.IsDisabled;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                // Manual logging for user status toggle
                await _loggingService.LogAsync(new CreateLogRequest
                {
                    Username = GetCurrentUsername(),
                    ActionType = user.IsDisabled ? "Ban" : "Unban",
                    EntityName = "User",
                    EntityId = id,
                    Description = $"{(user.IsDisabled ? "Banned" : "Unbanned")} user '{user.UserName}'",
                    OldValues = JsonConvert.SerializeObject(oldValues),
                    NewValues = JsonConvert.SerializeObject(new
                    {
                        UserName = user.UserName,
                        IsDisabled = user.IsDisabled,
                        Title = user.Title,
                        Roles = userRoles.ToList(),
                        UpdatedAt = DateTime.UtcNow
                    })
                });

                return Result.Success();
            }

            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, 0));
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

            // Store old values for logging
            var oldValues = new
            {
                UserName = user.UserName,
                CurrentRoles = currentRoles.ToList(),
                RolesCount = currentRoles.Count
            };

            if (rolesToRemove.Any())
                await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

            if (rolesToAdd.Any())
                await _userManager.AddToRolesAsync(user, rolesToAdd);

            var updatedRoles = await _userManager.GetRolesAsync(user);

            // Manual logging for user roles update
            await _loggingService.LogAsync(new CreateLogRequest
            {
                Username = GetCurrentUsername(),
                ActionType = "Update",
                EntityName = "User Roles",
                EntityId = request.UserId,
                Description = $"Updated roles for user '{user.UserName}' - Added: {rolesToAdd.Count}, Removed: {rolesToRemove.Count}",
                OldValues = JsonConvert.SerializeObject(oldValues),
                NewValues = JsonConvert.SerializeObject(new
                {
                    UserName = user.UserName,
                    UpdatedRoles = updatedRoles.ToList(),
                    RolesCount = updatedRoles.Count,
                    RolesAdded = rolesToAdd,
                    RolesRemoved = rolesToRemove,
                    UpdatedAt = DateTime.UtcNow
                })
            });

            return Result.Success();
        }

        public async Task<Result<UserPermissionResponse>> GetUserPermissionsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result.Failure<UserPermissionResponse>(UserErrors.UserNotFound);

            // Get user roles
            var userRoles = await _userManager.GetRolesAsync(user);

            // Get permissions from roles - ONLY role-based permissions
            var roleBasedPermissions = await _context.Roles
                .Join(_context.RoleClaims, role => role.Id, claim => claim.RoleId, (role, claim) => new { role, claim })
                .Where(x => userRoles.Contains(x.role.Name!))
                .Select(x => x.claim.ClaimValue)
                .Where(p => !string.IsNullOrEmpty(p))
                .Distinct()
                .ToListAsync();

            // Get ONLY individual user permissions
            var individualPermissions = await _context.UserPermissions
                .Where(up => up.UserId == userId)
                .Select(up => up.Permission)
                .ToListAsync();

            // Combine all permissions for the "AllPermissions" field
            var allPermissions = roleBasedPermissions
                .Union(individualPermissions)
                .Distinct()
                .ToList();

            var response = new UserPermissionResponse
            {
                UserId = userId,
                UserName = user.UserName!,
                IndividualPermissions = individualPermissions, // Only individual permissions
                RoleBasedPermissions = roleBasedPermissions!,  // Only role-based permissions  
                AllPermissions = allPermissions!               // Combined permissions
            };

            return Result.Success(response);
        }

        public async Task<Result> UpdateUserPermissionsAsync(UserPermissionRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return Result.Failure(UserErrors.UserNotFound);

            // Get current individual permissions
            var currentPermissions = await _context.UserPermissions
                .Where(up => up.UserId == request.UserId)
                .ToListAsync();

            // Store old values for logging
            var oldValues = new
            {
                UserId = request.UserId,
                UserName = user.UserName,
                IndividualPermissions = currentPermissions.Select(p => p.Permission).ToList()
            };

            // Remove all existing individual permissions
            _context.UserPermissions.RemoveRange(currentPermissions);

            // Add new individual permissions
            var newPermissions = request.Permissions.Select(permission => new UserPermission
            {
                UserId = request.UserId,
                Permission = permission,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            await _context.UserPermissions.AddRangeAsync(newPermissions);
            await _context.SaveChangesAsync();

            // Manual logging for user permissions update
            await _loggingService.LogAsync(new CreateLogRequest
            {
                Username = GetCurrentUsername(),
                ActionType = "Update",
                EntityName = "User Individual Permissions",
                EntityId = request.UserId,
                Description = $"Updated individual permissions for user '{user.UserName}' - Total: {request.Permissions.Count}",
                OldValues = JsonConvert.SerializeObject(oldValues),
                NewValues = JsonConvert.SerializeObject(new
                {
                    UserId = request.UserId,
                    UserName = user.UserName,
                    IndividualPermissions = request.Permissions,
                    UpdatedAt = DateTime.UtcNow
                })
            });

            return Result.Success();
        }
        public async Task<Result<IEnumerable<string>>> GetIndevedualPermissionsAsync()
        {
            var allPermissions = Permissions.GetIndividualPermissions();
            return Result.Success<IEnumerable<string>>(allPermissions!);
        }

        // User Profile (GET methods - no logging needed)

        public async Task<Result<UserResponse>> GetUserProfileByIdAsync(string userId)
        {
            var user = await _context.Users
                .Include(u => u.Department)
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
                return Result.Failure<UserResponse>(UserErrors.UserNotFound);

            var roles = await _userManager.GetRolesAsync(user);

            var response = new UserResponse
            {
                Id = user.Id,
                UserName = user.UserName!,
                IsDisabled = user.IsDisabled,
                InternalPhone = user.InternalPhone,
                UserPhone = user.PhoneNumber,
                Title = user.Title,
                DepartmentName = user.Department?.Name,
                Roles = roles,
                PhotoPath = user.PhotoPath,
            };

            return Result.Success(response);
        }

        public async Task<Result> ChangePasswordAsync(string userId, UserChangePasswordRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);

            var result = await _userManager.ChangePasswordAsync(user!, request.OldPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                int statusCode = StatusCodes.Status400BadRequest;
                return Result.Failure(new Error(error.Code, error.Description, statusCode));
            }

            // Manual logging for password change
            await _loggingService.LogAsync(new CreateLogRequest
            {
                Username = GetCurrentUsername(),
                ActionType = "Update",
                EntityName = "User Password",
                EntityId = userId,
                Description = $"User '{user.UserName}' changed their password",
                OldValues = null,
                NewValues = JsonConvert.SerializeObject(new
                {
                    UserName = user.UserName,
                    PasswordChangedAt = DateTime.UtcNow,
                    ChangedBy = "Self"
                })
            });

            return Result.Success();
        }

        public async Task<Result> AdminResetPasswordAsync(AdminResetPasswordRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return Result.Failure(UserErrors.UserNotFound);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

            if (!result.Succeeded)
            {
                int statusCode = StatusCodes.Status400BadRequest;
                var error = result.Errors.First();
                return Result.Failure(new Error(error.Code, error.Description, statusCode));
            }

            // Manual logging for admin password reset
            await _loggingService.LogAsync(new CreateLogRequest
            {
                Username = GetCurrentUsername(),
                ActionType = "Reset",
                EntityName = "User Password",
                EntityId = request.UserId,
                Description = $"Admin reset password for user '{user.UserName}'",
                OldValues = null,
                NewValues = JsonConvert.SerializeObject(new
                {
                    UserName = user.UserName,
                    PasswordResetAt = DateTime.UtcNow,
                    ResetBy = "Admin",
                    AdminUsername = GetCurrentUsername()
                })
            });

            return Result.Success();
        }

        public async Task<Result<UserResponse>> UpdateProfileAsync(string id, UpdateUserProfileRequest request)
        {
            var isExist = await _userManager.Users.AnyAsync(x => x.UserName == request.UserName && x.Id != id);
            if (isExist)
                return Result.Failure<UserResponse>(UserErrors.DublicatedUser);

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return Result.Failure<UserResponse>(UserErrors.UserNotFound);

            // Store old values for logging
            var oldDepartment = await _context.Departments.FindAsync(user.DepartmentId);
            var oldValues = new
            {
                UserName = user.UserName,
                Title = user.Title,
                DepartmentName = oldDepartment?.Name,
                InternalPhone = user.InternalPhone,
                UserPhone = user.PhoneNumber
            };

            if (!string.IsNullOrEmpty(request.UserName))
                user.UserName = request.UserName;

            if (!string.IsNullOrEmpty(request.Title))
                user.Title = request.Title;

            if (!string.IsNullOrEmpty(request.InternalPhone))
                user.InternalPhone = request.InternalPhone;

            if (!string.IsNullOrEmpty(request.USerPhone))
                user.PhoneNumber = request.USerPhone;

            Department? newDepartment = null;
            if (!string.IsNullOrEmpty(request.Department))
            {
                newDepartment = await _context.Departments.FirstOrDefaultAsync(x => x.Name == request.Department);
                if (newDepartment == null)
                    return Result.Failure<UserResponse>(DepartmentErrors.DepartmentNotFound);

                user.DepartmentId = newDepartment.Id;
            }

            await _userManager.UpdateAsync(user);

            var roles = await _userManager.GetRolesAsync(user);

            // Manual logging for profile update
            await _loggingService.LogAsync(new CreateLogRequest
            {
                Username = GetCurrentUsername(),
                ActionType = "Update",
                EntityName = "User Profile",
                EntityId = id,
                Description = $"Updated profile for user '{user.UserName}'",
                OldValues = JsonConvert.SerializeObject(oldValues),
                NewValues = JsonConvert.SerializeObject(new
                {
                    UserName = user.UserName,
                    Title = user.Title,
                    DepartmentName = newDepartment?.Name ?? oldDepartment?.Name,
                    InternalPhone = user.InternalPhone,
                    UserPhone = user.PhoneNumber,
                    UpdatedAt = DateTime.UtcNow
                })
            });

            return Result.Success(new UserResponse
            {
                Id = user.Id,
                UserName = user.UserName!,
                IsDisabled = user.IsDisabled,
                Title = user.Title,
                DepartmentName = request.Department,
                UserPhone = request.USerPhone,
                InternalPhone = request.InternalPhone,
                Roles = roles
            });
        }

        public async Task<Result<string>> UploadUserPhotoAsync(UploadUserPhotoRequest request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
                return Result.Failure<string>(UserErrors.UserNotFound);

            // CHANGED: Use network path instead of wwwroot
            var uploadsFolder = @"\\192.168.1.203\e$\App-data\user-photos";

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileExtension = Path.GetExtension(request.Photo!.FileName);
            var fileName = $"{Guid.NewGuid()}{fileExtension}"; // Generate unique filename
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Store old photo path for logging
            var oldPhotoPath = user.PhotoPath;

            // Delete old photo if it exists
            if (!string.IsNullOrEmpty(user.PhotoPath))
            {
                var oldFilePath = Path.Combine(uploadsFolder, user.PhotoPath);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            // Save the new photo to network location
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.Photo.CopyToAsync(stream);
            }

            // CHANGED: Store just the filename (not the path)
            user.PhotoPath = fileName;
            await _context.SaveChangesAsync();

            // Your logging code here...

            return Result.Success(fileName); // Return just filename
        }

        // Department methods (GET methods - no logging needed)

        public async Task<IEnumerable<DepartmentResponse>> GetAllDepartmentsAsync()
        {
            var departments = await _context.Departments.ToListAsync();
            var response = departments.Adapt<IEnumerable<DepartmentResponse>>();
            return response;
        }

        public async Task<Result<IEnumerable<UserResponse>>> GetAllDepartmentUsersAsync(int deparmentId)
        {
            var exsit = await _context.Departments.FindAsync(deparmentId);
            if (exsit == null)
            {
                return Result.Failure<IEnumerable<UserResponse>>(DepartmentErrors.DepartmentNotFound);
            }

            var users = await _context.Users.Include(x => x.Department).Where(d => d.DepartmentId == deparmentId).ToArrayAsync();
            var response = users.Adapt<IEnumerable<UserResponse>>();
            return Result.Success(response);
        }
    }
}