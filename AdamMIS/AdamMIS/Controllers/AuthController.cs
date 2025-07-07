using AdamMIS.Contract.Authentications;
using AdamMIS.Services.AuthServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdamMIS.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("")]

        public async Task<IActionResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
        {

            var authResult = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);

            if (authResult == null)
            {
                return BadRequest("Invalid Email/Password");
            }



            return Ok(authResult);
        }


    }
}
