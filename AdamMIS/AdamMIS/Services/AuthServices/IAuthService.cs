using AdamMIS.Contract.Authentications;

namespace AdamMIS.Services.AuthServices
{
    public interface IAuthService
    {
        Task<AuthResponse> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default);
    }
}
