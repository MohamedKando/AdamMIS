using AdamMIS.Abstractions;
using AdamMIS.Authentications;
using AdamMIS.Authentications.Filters;
using AdamMIS.Contract.Authentications;
using AdamMIS.Contract.UserRole;
using AdamMIS.Contract.Users;
using AdamMIS.Services.AuthServices;
using AdamMIS.Services.UsersServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AdamMIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService;
        public UserController(AppDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }
        [HttpGet("")]
      //  [HasPermission(Permissions.ReadUsers)]
        public async Task<IEnumerable<ApplicationUser>> GetAllUsers()
        {

            var user = await _userService.GetAllUsersAsync();
            return user;
        }

        [HttpGet("users-with-roles")]
      //  [HasPermission(Permissions.ReadUsers)]
        public async Task<IActionResult> GetAll()
        {

            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("banned-users")]
       // [HasPermission(Permissions.ReadUsers)]
        public async Task<IActionResult> GetAllBannedUsers()
        {

            var users = await _userService.GetAllBannedUsersAsync();
            return Ok(users);
        }

        [HttpGet("{userId}/roles")]
       // [HasPermission(Permissions.ReadRoles)]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            var roles = await _userService.GetUserRolesAsync(userId);
            return Ok(roles.Value);
        }

        [HttpPost("")]
       // [HasPermission(Permissions.RegisterUsers)]
        public async Task<IActionResult> AddUser(CreateUserRequest request)
        {

            var result = await _userService.AddUserAsync(request);
            if (!result.IsSuccess)
                return Problem(statusCode: result.Error.StatusCode, title: result.Error.Code, detail: result.Error.Description);
            return CreatedAtAction(nameof(GetAllUsers), new { result.Value!.Id }, result.Value);


        }

        [HttpPut("{id}")]
      //  [HasPermission(Permissions.DeleteUsers)]
        public async Task<IActionResult> ToggleStatues(string id)
        {

            var result = await _userService.ToggleStatusAsync(id);

            if (!result.IsSuccess)
                Problem(statusCode: result.Error.StatusCode, title: result.Error.Code, detail: result.Error.Description);
            return NoContent();
        }




        [HttpPut("role-update")]
       // [HasPermission(Permissions.UpdateRoles)]
        public async Task<IActionResult> UpdateUserRoles([FromBody] UserRoleRequest request)
        {
            
            var result = await _userService.UpdateUserRolesAsync(request);
            if (result.IsSuccess)
            {
                return NoContent();
            }

            return Problem(statusCode: result.Error.StatusCode, title: result.Error.Code, detail: result.Error.Description);
        }



        [HttpGet("departments")]
        public async Task<IEnumerable<string>> GetAllDepartment()
        {
            var departments = await _userService.GetAllDepartmentsAsync();
            return departments;
    
        }












        // User Profile


        [HttpGet("{id}")]
       [Authorize]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _userService.GetUserProfileByIdAsync(id);
            if (!result.IsSuccess)
                return Problem(result.Error.Description);
            return Ok(result.Value);
        }

        [HttpPost("reset-password")]
       // [HasPermission(Permissions.UpdateUsers)]
        public async Task<IActionResult> ResetPassword([FromBody] AdminResetPasswordRequest request)
        {
            var result = await _userService.AdminResetPasswordAsync(request);
            if (!result.IsSuccess)
                return Problem(statusCode: result.Error.StatusCode, title: result.Error.Code, detail: result.Error.Description);
            return NoContent();
        }




        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword( [FromBody] UserChangePasswordRequest request)
        {
            var result = await _userService.ChangePasswordAsync(User.GetUserId(), request);
            if (!result.IsSuccess)
                return Problem(statusCode: result.Error.StatusCode, title: result.Error.Code, detail: result.Error.Description);
            return NoContent();
        }

        [HttpPut("update-profile/{id}")]
        //[Authorize]
        public async Task<IActionResult> UpdateProfile(string id, UpdateUserProfileRequest request)
        {
            var result = await _userService.UpdateProfileAsync(id, request);
            if (!result.IsSuccess)
                return Problem(statusCode: result.Error.StatusCode, title: result.Error.Code, detail: result.Error.Description);
            return Ok(result.Value);
        }

        [HttpPost("UploadPhoto")]
        public async Task<ActionResult> UploadPhoto([FromForm] UploadUserPhotoRequest model)
        {
            var result = await _userService.UploadUserPhotoAsync(model);
            if (result.IsSuccess)
                return Ok(new { photoPath = result.Value });
            return  NoContent();
        }

    }


}
