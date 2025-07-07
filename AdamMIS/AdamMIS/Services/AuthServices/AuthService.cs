using AdamMIS.Authentications;
using AdamMIS.Contract.Authentications;
using Microsoft.AspNetCore.Identity;

namespace AdamMIS.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtProvider _jwtProvider;
        public AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider)
        {
            _userManager = userManager;
            _jwtProvider = jwtProvider;
        }
        public async Task<AuthResponse> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            //checking if the user exist in the database or not
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return null;
            }
            // check password 

            var isValidPassword =await _userManager.CheckPasswordAsync(user, password);
            if (isValidPassword == false)
            {
                return null;
            }

            //generate jwt 
            var (token, expireIn) = _jwtProvider.GenerateToken(user);

            // returning the response

            return new AuthResponse
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = token,
                ExpiresIn = expireIn*60
            };
        }
    }
}
