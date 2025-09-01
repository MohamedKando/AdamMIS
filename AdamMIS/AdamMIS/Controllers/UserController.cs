using AdamMIS.Abstractions;
using AdamMIS.Authentications;
using AdamMIS.Authentications.Filters;
using AdamMIS.Contract.Authentications;
using AdamMIS.Contract.Departments;
using AdamMIS.Contract.UserRole;
using AdamMIS.Contract.Users;
using AdamMIS.Entities.UserEntities;
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




        [HttpGet("user-permissions/{userId}")]
       // [HasPermission(Permissions.ReadUsers)]
        public async Task<IActionResult> GetUserPermissions(string userId)
        {
            var result = await _userService.GetUserPermissionsAsync(userId);
            return result.IsSuccess ? Ok(result.Value) : Problem(result.Error.Description);
        }

        [HttpPut("update-permissions")]
      //  [HasPermission(Permissions.UpdateUsers)]
        public async Task<IActionResult> UpdateUserPermissions(UserPermissionRequest request)
        {
            var result = await _userService.UpdateUserPermissionsAsync(request);
            return result.IsSuccess ? Ok() : Problem(result.Error.Description);
        }
        [HttpGet("Indevedual-permissions")]
       // [HasPermission(Permissions.ReadUsers)]
        public async Task<IActionResult> GetIndevedualPermissions()
        {
            var result = await _userService.GetIndevedualPermissionsAsync();
            return result.IsSuccess ? Ok(result.Value) : Problem(result.Error.Description);
        }





        // User Profile


        [HttpGet("{id}")]
       //[Authorize]
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








        //departments 
        [HttpGet("departments")]

        public async Task<IEnumerable<DepartmentResponse>> GetAllDepartment()
        {
            var departments = await _userService.GetAllDepartmentsAsync();
            return departments;

        }


        [HttpGet("department-users/{categoryId}")]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetAllDepartmentUsers(int categoryId)
        {
            var result = await _userService.GetAllDepartmentUsersAsync(categoryId);

            if (result.IsFailure)
                return Problem(result.Error.Description,result.Error.Code,result.Error.StatusCode);

            return Ok(result.Value);
        }

        [HttpGet("department-users/assigen-head")]
        public async Task<ActionResult> AssigenUserToDepartmentHead(DepartmentHeadRequest request)
        {
            var result = await _userService.AssignUserAsDepartmentHeadAsync(request);

            if (result.IsFailure)
                return Problem(result.Error.Description, result.Error.Code, result.Error.StatusCode);

            return NoContent();
        }

        [HttpGet("department-users/remove-head")]
        public async Task<ActionResult> RemoveUserFromDepartmentHead(DepartmentHeadRequest request)
        {
            var result = await _userService.RemoveUserAsDepartmentHeadAsync(request);

            if (result.IsFailure)
                return Problem(result.Error.Description, result.Error.Code, result.Error.StatusCode);

            return NoContent();
        }





    }


}
