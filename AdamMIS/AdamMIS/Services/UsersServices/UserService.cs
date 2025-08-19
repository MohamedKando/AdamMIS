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
                }
            ).ToListAsync();

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

            // Use the network path for storing photos
            var uploadsFolder = @"\\192.168.1.203\e$\App-data\user-photos";

            try
            {
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);
            }
            catch (Exception ex)
            {
                // Handle network path access issues
                return Result.Failure<string>(UserErrors.UserNotFound);
            }

            var fileExtension = Path.GetExtension(request.Photo!.FileName);
            var fileName = $"{request.UserId}{fileExtension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Store old photo path for logging
            var oldPhotoPath = user.PhotoPath;

            // Delete old photo if it exists
            if (!string.IsNullOrEmpty(user.PhotoPath))
            {
                // For network path, construct the full path differently
                var oldFileName = Path.GetFileName(user.PhotoPath);
                var oldFilePath = Path.Combine(uploadsFolder, oldFileName);

                if (System.IO.File.Exists(oldFilePath))
                {
                    try
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                    catch (Exception ex)
                    {
                        // Log the error but continue with upload
                        await _loggingService.LogAsync(new CreateLogRequest
                        {
                            Username = GetCurrentUsername(),
                            ActionType = "Warning",
                            EntityName = "User Photo",
                            EntityId = request.UserId,
                            Description = $"Failed to delete old photo: {ex.Message}"
                        });
                    }
                }
            }

            // Save the new photo
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.Photo.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
                return Result.Failure<string>(NetworkErrors.NetworkNotFound);
            }

            // Update user's photo path in DB - store the full network path
            var relativePath = filePath; // Store the complete network path
            user.PhotoPath = relativePath;
            await _context.SaveChangesAsync();

            // Manual logging for photo upload
            await _loggingService.LogAsync(new CreateLogRequest
            {
                Username = GetCurrentUsername(),
                ActionType = "Update",
                EntityName = "User Photo",
                EntityId = request.UserId,
                Description = $"Updated photo for user '{user.UserName}' to network storage",
                OldValues = JsonConvert.SerializeObject(new
                {
                    UserName = user.UserName,
                    OldPhotoPath = oldPhotoPath,
                    HasOldPhoto = !string.IsNullOrEmpty(oldPhotoPath)
                }),
                NewValues = JsonConvert.SerializeObject(new
                {
                    UserName = user.UserName,
                    NewPhotoPath = relativePath,
                    NetworkPath = filePath,
                    FileName = request.Photo.FileName,
                    FileSizeBytes = request.Photo.Length,
                    FileExtension = fileExtension,
                    UploadedAt = DateTime.UtcNow
                })
            });

            return Result.Success(relativePath);
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