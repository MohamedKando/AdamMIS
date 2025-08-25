using AdamMIS.Services.GLPIServices.TicketingServices;

namespace AdamMIS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketingService _glpiService;

        public TicketsController(ITicketingService glpiService)
        {
            _glpiService = glpiService;

        }
        [HttpGet]
        public async Task<IActionResult> GetAllTickets()
        {
            try
            {
                var result = await _glpiService.GetAllTicketsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving tickets: {ex.Message}");
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicket ticketDto)
        {
            try
            {
                // Simple validation
                if (string.IsNullOrEmpty(ticketDto.Title) || string.IsNullOrEmpty(ticketDto.Description))
                {
                    return BadRequest("Title and description are required");
                }

                var result = await _glpiService.CreateTicketAsync(ticketDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating ticket: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicket(int id)
        {
            try
            {
                var result = await _glpiService.GetTicketAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving ticket: {ex.Message}");
            }
        }

        // Keep your original simple endpoint for backward compatibility
        [HttpPost("create-simple")]
        public async Task<IActionResult> CreateSimpleTicket([FromForm] string title, [FromForm] string description)
        {
            var ticketDto = new CreateTicket
            {
                Title = title,
                Description = description
            };

            return await CreateTicket(ticketDto);
        }
        [HttpPatch("{id}/solve")]
        public async Task<IActionResult> SolveTicket(int id)
        {
            try
            {
                var result = await _glpiService.SolveTicketAsync(id);
                return Ok(new
                {
                    message = "Ticket solved successfully",
                    ticket = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error solving ticket: {ex.Message}");
            }
        }

        [HttpPatch("{id}/close")]
        public async Task<IActionResult> CloseTicket(int id)
        {
            try
            {
                var result = await _glpiService.CloseTicketAsync(id);
                return Ok(new
                {
                    message = "Ticket closed successfully",
                    ticket = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error closing ticket: {ex.Message}");
            }
        }
    }
}
