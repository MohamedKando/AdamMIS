using AdamMIS.Contract.UserRole;
using AdamMIS.Contract.Users;

namespace AdamMIS.Services.UsersServices
{
    public interface IUserService
    {
        Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
        Task<IEnumerable<UserResponse>> GetAllAsync();
        Task<Result<IEnumerable<string>>> GetUserRolesAsync(string userId);
        Task<Result<UserResponse>> AddUserAsync(CreateUserRequest request);
        Task<IEnumerable<UserResponse>> GetAllBannedUsersAsync();
        Task<Result> ToggleStatusAsync(string id);
        Task<Result> UpdateUserRolesAsync(UserRoleRequest request);

        Task<IEnumerable<string>> GetAllDepartmentsAsync();

        //User Profile
        Task<Result<UserResponse>> GetUserProfileByIdAsync(string userId);
        Task<Result> AdminResetPasswordAsync(AdminResetPasswordRequest request);
        Task<Result> ChangePasswordAsync(string userId ,UserChangePasswordRequest request);

        Task<Result<UserResponse>> UpdateProfileAsync(string id, UpdateUserProfileRequest request);
        Task<Result<string>> UploadUserPhotoAsync(UploadUserPhotoRequest request);



    }
}
