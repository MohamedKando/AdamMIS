using AdamMIS.Abstractions;
using AdamMIS.Authentications;
using AdamMIS.Contract.Authentications;
using AdamMIS.Contract.SystemLogs;
using AdamMIS.Entities.SystemLogs;
using AdamMIS.Services.AuthServices;
using AdamMIS.Services.LogServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdamMIS.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public readonly IAuthService _authService;
        private readonly ILoggingService _logginservice;
        private readonly AppDbContext _context;
        public AuthController(IAuthService authService, ILoggingService loggingService, AppDbContext context)
        {
            _authService = authService;
            _logginservice = loggingService;
            _context = context;
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
        [HttpPost("logout")]
        [Authorize] // Ensure user is authenticated to logout
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            var ip = HttpContext.Request?.Headers["X-Forwarded-For"].FirstOrDefault()
            ?? HttpContext.Connection?.RemoteIpAddress?.MapToIPv4().ToString()
            ?? "Unknown";
            try
            {
                // Get current user info from the JWT token
                var username = User.Identity?.Name ?? "Unknown";
                var userId = User.GetUserId();

                await _logginservice.LogAsync(new CreateLogRequest
                {
                    Username = username,
                    ActionType = "Logout",
                    EntityName = "Authentication",
                    EntityId = userId,
                    Description = "User logged out",
                    IpAddress = ip
                });

                var log = await _context.acivityLogs
                    .FirstOrDefaultAsync(l => l.UserId == userId && l.IsOnline, cancellationToken);


                if (log != null)
                {
                    log.LastActivityTime = DateTime.UtcNow;
                    log.IsOnline = false;
                    log.SessionTime = log.LastActivityTime-log.LoginTime;
                    await _context.SaveChangesAsync();
                }

                return Ok(new { message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                // Log the error but still return success since logout should work
                // even if logging fails
                return Ok(new { message = "Logged out successfully" });
            }
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

        [HttpPost("heartbeat")]
        //[Authorize]
        public async Task<IActionResult> Heartbeat()
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            var log = await _context.acivityLogs
                .FirstOrDefaultAsync(l => l.UserId == userId /*&& l.IsOnline*/);

            if (log != null)
            {
                log.LastActivityTime = DateTime.UtcNow;
                log.SessionTime = log.LastActivityTime - log.LoginTime;
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Heartbeat received" });
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
