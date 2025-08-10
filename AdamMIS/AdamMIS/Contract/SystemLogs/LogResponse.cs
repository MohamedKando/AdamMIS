namespace AdamMIS.Contract.SystemLogs
{
    public class LogResponse
    {
        public long Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string ActionType { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public DateTime Timestamp { get; set; }
        public string? IpAddress { get; set; }
    }
}
