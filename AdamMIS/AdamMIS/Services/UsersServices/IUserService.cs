using AdamMIS.Contract.UserRole;
using AdamMIS.Contract.Users;

namespace AdamMIS.Services.UsersServices
{
    public interface IUserService
    {
        Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
        Task<IEnumerable<UserResponse>> GetAllAsync();
        Task<Result<UserResponse>> AddUserAsync(CreateUserRequest request);
        Task<IEnumerable<UserResponse>> GetAllBannedUsersAsync();
        Task<Result> ToggleStatusAsync(string id);

        Task<Result<IEnumerable<UserRoleResponse>>> AssignRolesToUserAsync(UserRoleRequest request, string assignedBy);
        Task<Result> RemoveRoleFromUserAsync(UserRoleRequest request);
        //Task<IEnumerable<UserRoleResponse>> GetUserRoleAssignmentsAsync(string userId);

    }
}
