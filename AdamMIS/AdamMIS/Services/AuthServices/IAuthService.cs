

namespace AdamMIS.Services.AuthServices
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default);
        Task<Result<AuthResponse>> RigesterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
        Task<Result> ClearAllUsersAsync(CancellationToken cancellationToken=default);
    }
}
