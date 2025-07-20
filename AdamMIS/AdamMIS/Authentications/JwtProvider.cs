using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AdamMIS.Authentications
{
    public class JwtProvider : IJwtProvider
    {
        private protected readonly JwtOptions _jwtOptions;

        public JwtProvider(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;

        }
        public (string token, int expiresIn) GenerateToken(ApplicationUser user)
        {
            Claim[] claims = [

                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(ClaimTypes.Name,user.UserName!),
               
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            ];

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var expiresIn = _jwtOptions.ExpiryMinutes;

            var expirationDate = DateTime.UtcNow.AddMinutes(expiresIn);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: expirationDate,
                signingCredentials: signingCredentials

            );
            return (token: new JwtSecurityTokenHandler().WriteToken(token), expiresIn: expiresIn);
        }

        public string ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    IssuerSigningKey = symmetricSecurityKey,
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;
                return jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
            }
            catch
            {
                return null!;
            }
        }
    }
}
