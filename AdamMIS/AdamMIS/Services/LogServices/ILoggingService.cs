
namespace AdamMIS.Services.LogServices
{
    public interface ILoggingService
    {
        Task LogAsync(CreateLogRequest dto);
        Task<PaginatedList<LogResponse>> GetLogsAsync(
                    string? username,
                    string? actionType,
                    DateTime? startDate,
                    DateTime? endDate,
                    RequestFilters filters
                );
        Task<Result> ClearAllLogsAsync(CancellationToken cancellationToken);
    }
}
