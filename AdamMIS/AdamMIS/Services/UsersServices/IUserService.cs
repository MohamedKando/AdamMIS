
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

        // New methods for individual permissions
        Task<Result<UserPermissionResponse>> GetUserPermissionsAsync(string userId);
        Task<Result> UpdateUserPermissionsAsync(UserPermissionRequest request);
        Task<Result<IEnumerable<string>>> GetIndevedualPermissionsAsync();

        //User Profile
        Task<Result<UserResponse>> GetUserProfileByIdAsync(string userId);
        Task<Result> AdminResetPasswordAsync(AdminResetPasswordRequest request);
        Task<Result> ChangePasswordAsync(string userId, UserChangePasswordRequest request);
        Task<Result<UserResponse>> UpdateProfileAsync(string id, UpdateUserProfileRequest request);
        Task<Result<string>> UploadUserPhotoAsync(UploadUserPhotoRequest request);

        //departments
        Task<IEnumerable<DepartmentResponse>> GetAllDepartmentsAsync();
        Task<Result<IEnumerable<UserResponse>>> GetAllDepartmentUsersAsync(int deparmentId);
    }
}