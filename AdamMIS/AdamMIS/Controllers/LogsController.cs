using AdamMIS.Contract.Common;
using AdamMIS.Contract.SystemLogs;
using AdamMIS.Services.LogServices;

namespace AdamMIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : Controller
    {
        private readonly ILoggingService _loggingService;
        private readonly AppDbContext _context;

        public LogsController(ILoggingService loggingService, AppDbContext appDbContext)
        {
            _loggingService = loggingService;
            _context = appDbContext;
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateLog([FromBody] CreateLogRequest dto)
        {
            await _loggingService.LogAsync(dto);
            return Ok(new { message = "Log created successfully" });
        }

        [HttpGet("")]
        public async Task<IActionResult> GetLogs(
            [FromQuery] string? username,
            [FromQuery] string? actionType,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] RequestFilters filters)
        {
            var logs = await _loggingService.GetLogsAsync(username, actionType, startDate, endDate,filters);
            return Ok(logs);
        }


        [HttpGet("activity-logs")]
        public async Task<IActionResult> GetActivitylogs()
        {
            var logs = await _context.acivityLogs.ToListAsync();
            return Ok(logs);
        }




        [HttpDelete("ClearAll")]
        public async Task<IActionResult> ClearLogs(CancellationToken cancellationToken)
        {

            var result = await _loggingService.ClearAllLogsAsync(cancellationToken);

            if (result.IsFailure)
            {
                return Problem(statusCode: result.Error.StatusCode, title: result.Error.Code, detail: result.Error.Description);

            }
            else
                return NoContent();

        }
    }
}
