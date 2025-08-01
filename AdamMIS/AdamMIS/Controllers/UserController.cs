﻿using AdamMIS.Abstractions;
using AdamMIS.Contract.Authentications;
using AdamMIS.Contract.UserRole;
using AdamMIS.Contract.Users;
using AdamMIS.Services.AuthServices;
using AdamMIS.Services.UsersServices;
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
        public async Task<IEnumerable<ApplicationUser>> GetAllUsers()
        {

            var user = await _userService.GetAllUsersAsync();
            return user;
        }

        [HttpGet("users-with-roles")]
        public async Task<IActionResult> GetAll()
        {

            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("banned-users")]
        public async Task<IActionResult> GetAllBannedUsers()
        {

            var users = await _userService.GetAllBannedUsersAsync();
            return Ok(users);
        }

        [HttpGet("{userId}/roles")]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            var roles = await _userService.GetUserRolesAsync(userId);
            return Ok(roles.Value);
        }

        [HttpPost("")]
        public async Task<IActionResult> AddUser(CreateUserRequest request)
        {

            var result = await _userService.AddUserAsync(request);
            if (!result.IsSuccess)
                return Problem(statusCode: StatusCodes.Status400BadRequest, title: result.Error.Code, detail: result.Error.Description);
            return CreatedAtAction(nameof(GetAllUsers), new { result.Value!.Id }, result.Value);


        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ToggleStatues(string id)
        {

            var result = await _userService.ToggleStatusAsync(id);

            if (!result.IsSuccess)
                Problem(statusCode: StatusCodes.Status400BadRequest, title: result.Error.Code, detail: result.Error.Description);
            return NoContent();
        }




        [HttpPut("role-update")]
        public async Task<IActionResult> UpdateUserRoles([FromBody] UserRoleRequest request)
        {
            
            var result = await _userService.UpdateUserRolesAsync(request);
            if (result.IsSuccess)
            {
                return NoContent();
            }

            return Problem(statusCode: StatusCodes.Status400BadRequest, title: result.Error.Code, detail: result.Error.Description);
        }


       
    }


}
