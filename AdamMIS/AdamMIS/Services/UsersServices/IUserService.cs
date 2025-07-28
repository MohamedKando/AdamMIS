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
    }
}
