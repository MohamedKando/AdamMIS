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
        //Task<IEnumerable<UserRoleResponse>> GetUserRoleAssignmentsAsync(string userId);

    }
}
