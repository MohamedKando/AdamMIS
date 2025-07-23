using AdamMIS.Contract.Roles;
using AdamMIS.Services.RolesServices;
using Azure;

namespace AdamMIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAll([FromQuery]bool? includeDisabled )
        {
            var response = await _roleService.GetAllAsync(includeDisabled);

            return Ok(response);
        }

        [HttpGet("role-details/{id}")]
         
        public async Task<IActionResult> GetRoleDetails([FromRoute] string id)
        {
            var roleDetail = await _roleService.GetRolesDetailsAsync(id);

            return Ok(roleDetail);

        }

        [HttpPost("")]
        public async Task<IActionResult> AddRole([FromBody] RoleRequest request)
        {
            var result = await _roleService.AddRoleAsync(request);
            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetAll), new { result.Value!.Id },result.Value);
            }
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: result.Error.Code, detail: result.Error.Description);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole([FromRoute] string id,[FromBody] RoleRequest request)
        {
            var result = await _roleService.UpdateRoleAsync(id,request);
            return Ok(result);
        }

        [HttpPut("Toggle/{id}")]
        public async Task<IActionResult> ToggleRole([FromRoute] string id)
        {
            var result = await _roleService.ToggleStatusAsync(id);
            
            return Ok(result);
        }
    }
}
