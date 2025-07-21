using AdamMIS.Authentications;
using AdamMIS.Contract.Authentications;
using AdamMIS.Errors;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AdamMIS.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtProvider _jwtProvider;
        private readonly AppDbContext _context;
        public AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider, AppDbContext context)
        {
            _userManager = userManager;
            _jwtProvider = jwtProvider;
            _context = context;
        }
        public async Task<Result<AuthResponse>> GetTokenAsync(string name, string password, CancellationToken cancellationToken = default)
        {
            //checking if the user exist in the database or not
            var user = await _userManager.FindByNameAsync(name);
            if (user == null)
            {
                return Result.Failure<AuthResponse>(UserErrors.UserInvalidCredentials);
            }
            // check password 

            var isValidPassword =await _userManager.CheckPasswordAsync(user, password);
            if (isValidPassword == false)
            {
             return Result.Failure<AuthResponse>(UserErrors.UserInvalidCredentials);
            }

            //generate jwt 
            var (userRoles, userPermissions) = await GetUserRolesAndPermissions(user);
            var (token, expireIn) = _jwtProvider.GenerateToken(user, userRoles, userPermissions!);

            // returning the response

            var response = new AuthResponse
            {
                Id = user.Id,
                UserName = user.UserName!,
                //FirstName = user.FirstName,
                //LastName = user.LastName,
                Token = token,
                ExpiresIn = expireIn * 60
            };
            return Result<AuthResponse>.Success(response);
        }
        public async Task<Result<AuthResponse>> RigesterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        { 
            var userIsExists = await _userManager.Users.AnyAsync(x=>x.UserName== request.UserName);
            if (userIsExists == true)
            {
                return Result.Failure<AuthResponse>(UserErrors.DublicatedUser);
            }

            var user = request.Adapt<ApplicationUser>();

            var result = await _userManager.CreateAsync(user,request.Password);
            if(result.Succeeded)
            {
                var response = new AuthResponse
                {
                    Id = user.Id,
                    UserName = request.UserName
                };
                await _userManager.AddToRoleAsync(user, DeafultRole.Member);
                return  Result.Success(response);
            }
            var error = result.Errors.First();
            return Result.Failure<AuthResponse>(new Error (error.Code,error.Description));
        }
        public async Task<Result> ClearAllUsersAsync(CancellationToken cancellationToken)
        {
            // 1. Delete all users
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [AspNetUsers]", cancellationToken);

            // 2. Reset identity seed (to start from 1)
            

            return Result.Success();
        }
        private async Task<(IEnumerable<string>roles,IEnumerable<string> permissions)> GetUserRolesAndPermissions(ApplicationUser user)
        {

            var userRoles = await _userManager.GetRolesAsync(user);
            var userPermissions = await _context.Roles
                .Join(_context.RoleClaims,role=>role.Id,claim=>claim.RoleId,(role,claim)=>new {role,claim})
                .Where(x=>userRoles.Contains(x.role.Name!)).Select(x=>x.claim.ClaimValue).Distinct().ToListAsync();
            return (userRoles,userPermissions!);
        }

    }
}
