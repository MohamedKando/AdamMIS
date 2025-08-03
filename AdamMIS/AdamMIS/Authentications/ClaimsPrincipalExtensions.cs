using System.Security.Claims;

namespace AdamMIS.Authentications
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier); // usually the ID claim
        }
    }
}
