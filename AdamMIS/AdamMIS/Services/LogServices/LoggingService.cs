

namespace AdamMIS.Services.LogServices
{
    public class LoggingService : ILoggingService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public LoggingService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAsync(CreateLogRequest dto)
        {
            var ip = _httpContextAccessor.HttpContext?.Request?.Headers["X-Forwarded-For"].FirstOrDefault()
         ?? _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.MapToIPv4().ToString()
         ?? "Unknown";
            var log = new SystemLog
            {
                Username = dto.Username,
                ActionType = dto.ActionType,
                EntityName = dto.EntityName,
                EntityId = dto.EntityId,
                Description = dto.Description,
                OldValues = dto.OldValues,
                NewValues = dto.NewValues,
                Timestamp = DateTime.Now,
                IpAddress = ip
            };

            _context.SystemLog.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task<PaginatedList<LogResponse>> GetLogsAsync(
            string? username,
            string? actionType,
            DateTime? startDate,
            DateTime? endDate,
            RequestFilters filters
        )
        {
            var query = _context.SystemLog.AsQueryable();

            if (!string.IsNullOrEmpty(username))
                query = query.Where(l => l.Username.ToLower() == username.ToLower());

            if (!string.IsNullOrEmpty(actionType))
                query = query.Where(l => l.ActionType.ToLower() == actionType.ToLower());

            if (startDate.HasValue)
                query = query.Where(l => l.Timestamp >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(l => l.Timestamp <= endDate.Value);

            var projectedQuery = query
                .OrderByDescending(l => l.Timestamp)
                .Select(l => new LogResponse
                {
                    Id = l.Id,
                    Username = l.Username,
                    ActionType = l.ActionType,
                    EntityName = l.EntityName,
                    EntityId = l.EntityId,
                    Description = l.Description,
                    OldValues = l.OldValues,
                    NewValues = l.NewValues,
                    Timestamp = l.Timestamp,
                    IpAddress = l.IpAddress
                });

            return await PaginatedList<LogResponse>.CreateAsync(projectedQuery, filters.PageNumber, filters.PageSize);
        }



        public async Task<Result> ClearAllLogsAsync(CancellationToken cancellationToken)
        {
            // 1. Delete all users
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [SystemLog]", cancellationToken);

            // 2. Reset identity seed (to start from 1)
            await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('SystemLog', RESEED, 0)", cancellationToken);

            return Result.Success();
        }
    }
}
