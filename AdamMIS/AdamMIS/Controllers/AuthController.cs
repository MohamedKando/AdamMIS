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

        public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
        {

            var authResult = await _authService.GetTokenAsync(request.UserName, request.Password, cancellationToken);

            if (authResult.IsFailure)
            {
                return Problem(statusCode: StatusCodes.Status400BadRequest,title:authResult.Error.Code,detail:authResult.Error.Description);
            }



            return Ok(authResult.Value);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            var authResult = await _authService.RigesterAsync(request, cancellationToken);
            if (authResult.IsFailure)
            {
                return Problem(statusCode: StatusCodes.Status400BadRequest, title: authResult.Error.Code, detail: authResult.Error.Description);
            }
            else
                return Ok(authResult.Value);

        }
        [HttpDelete("ClearAll")]
        public async Task<IActionResult> ClearUsers(CancellationToken cancellationToken)
        {

            var result = await _authService.ClearAllUsersAsync(cancellationToken);

            if (result.IsFailure)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.Code, detail: result.Error.Description);

            }
            else
                return NoContent();

        }
    }
}
