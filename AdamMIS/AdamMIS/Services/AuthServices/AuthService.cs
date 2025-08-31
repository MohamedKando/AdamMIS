

namespace AdamMIS.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtProvider _jwtProvider;
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider, AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _jwtProvider = jwtProvider;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<AuthResponse>> GetTokenAsync(string name, string password, CancellationToken cancellationToken = default)
        {
            var ip = _httpContextAccessor.HttpContext?.Request?.Headers["X-Forwarded-For"].FirstOrDefault()
         ?? _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.MapToIPv4().ToString()
         ?? "Unknown";

            //checking if the user exist in the database or not
            var user = await _userManager.FindByNameAsync(name);
            if (user == null)
            {
                return Result.Failure<AuthResponse>(UserErrors.UserInvalidCredentials);
            }

            // check password and activity
            var isValidPassword = await _userManager.CheckPasswordAsync(user, password);
            if (isValidPassword == false)
            {
                return Result.Failure<AuthResponse>(UserErrors.UserInvalidCredentials);
            }

            if (user.IsDisabled)
            {
                return Result.Failure<AuthResponse>(UserErrors.UserDisabled);
            }

            //generate jwt with both role-based and individual permissions
            var (userRoles, userPermissions) = await GetUserRolesAndPermissions(user);
            var (token, expireIn) = _jwtProvider.GenerateToken(user, userRoles, userPermissions);

            // returning the response
            var response = new AuthResponse
            {
                Id = user.Id,
                UserName = user.UserName!,
                Token = token,
                ExpiresIn = expireIn * 600
            };

            var existingLog = await _context.acivityLogs
                .FirstOrDefaultAsync(l => l.UserId == user.Id, cancellationToken);

            if (existingLog == null)
            {
                var log = new AcivityLogs
                {
                    UserId = user.Id,
                    UserName = name,
                    LoginTime = DateTime.UtcNow,
                    LastActivityTime = DateTime.UtcNow,
                    IsOnline = true,
                    IpAddress = ip
                };
                _context.acivityLogs.Add(log);
            }
            else
            {
                existingLog.LastActivityTime = DateTime.UtcNow;
                existingLog.IsOnline = true;
                existingLog.LoginTime = DateTime.UtcNow;
                existingLog.SessionTime = existingLog.LoginTime - existingLog.LastActivityTime;
                existingLog.IpAddress = ip;
            }

            await _context.SaveChangesAsync();
            return Result<AuthResponse>.Success(response);
        }

        public async Task<Result<AuthResponse>> RigesterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            var userIsExists = await _userManager.Users.AnyAsync(x => x.UserName == request.UserName);
            if (userIsExists == true)
            {
                return Result.Failure<AuthResponse>(UserErrors.DublicatedUser);
            }

            var user = request.Adapt<ApplicationUser>();

            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                var response = new AuthResponse
                {
                    Id = user.Id,
                    UserName = request.UserName
                };
                await _userManager.AddToRoleAsync(user, DeafultRole.Member);
                return Result.Success(response);
            }
            var error = result.Errors.First();
            return Result.Failure<AuthResponse>(new Error(error.Code, error.Description, 0));
        }

        public async Task<Result> ClearAllUsersAsync(CancellationToken cancellationToken)
        {
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [AspNetUsers]", cancellationToken);
            return Result.Success();
        }

        // THIS IS THE KEY METHOD - Make sure it includes individual permissions
        private async Task<(IEnumerable<string> roles, IEnumerable<string> permissions)> GetUserRolesAndPermissions(ApplicationUser user)
        {
            // Get user roles
            var userRoles = await _userManager.GetRolesAsync(user);

            // Get permissions from roles
            var roleBasedPermissions = await _context.Roles
                .Join(_context.RoleClaims, role => role.Id, claim => claim.RoleId, (role, claim) => new { role, claim })
                .Where(x => userRoles.Contains(x.role.Name!))
                .Where(x => x.claim.ClaimValue != null)
                .Select(x => x.claim.ClaimValue!)
                .ToListAsync();

            // Get individual user permissions from UserPermissions table
            var individualPermissions = await _context.UserPermissions
                .Where(up => up.UserId == user.Id)
                .Select(up => up.Permission)
                .ToListAsync();

            // Combine role-based permissions and individual permissions, removing duplicates
            var allPermissions = roleBasedPermissions
                .Union(individualPermissions)
                .Where(p => !string.IsNullOrEmpty(p))
                .Distinct()
                .ToList();

            return (userRoles, allPermissions);
        }
    }
}