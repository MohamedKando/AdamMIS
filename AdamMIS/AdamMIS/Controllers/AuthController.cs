using AdamMIS.Abstractions;
using AdamMIS.Contract.Authentications;
using AdamMIS.Contract.SystemLogs;
using AdamMIS.Entities.SystemLogs;
using AdamMIS.Services.AuthServices;
using AdamMIS.Services.LogServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdamMIS.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public readonly IAuthService _authService;
        private readonly ILoggingService _logginservice;
        public AuthController(IAuthService authService, ILoggingService loggingService)
        {
            _authService = authService;
            _logginservice = loggingService;
        }
        [HttpPost("")]

        public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
        {

            var authResult = await _authService.GetTokenAsync(request.UserName, request.Password, cancellationToken);


            if (authResult.IsFailure)
            {
                return Problem(statusCode: authResult.Error.StatusCode, title:authResult.Error.Code,detail:authResult.Error.Description);
            }
            await _logginservice.LogAsync(new CreateLogRequest
            {
                Username = request.UserName,
                ActionType = "Login",
                EntityName = "Authentication",
                EntityId = "N/A",
                Description = "User logged in",
                
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            });


            return Ok(authResult.Value);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            var authResult = await _authService.RigesterAsync(request, cancellationToken);
            if (authResult.IsFailure)
            {
                return Problem(statusCode: authResult.Error.StatusCode, title: authResult.Error.Code, detail: authResult.Error.Description);
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
                return Problem(statusCode: result.Error.StatusCode, title: result.Error.Code, detail: result.Error.Description);

            }
            else
                return NoContent();

        }
    }
}
