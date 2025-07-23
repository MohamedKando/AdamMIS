using AdamMIS.Contract.Authentications;
using AdamMIS.Services.AuthServices;
using AdamMIS.Services.UsersServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdamMIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService;
        public UserController(AppDbContext context , IUserService userService)
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
    }


}
