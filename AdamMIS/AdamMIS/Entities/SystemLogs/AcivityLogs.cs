namespace AdamMIS.Entities.SystemLogs
{
    public class AcivityLogs
    {
        public long Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty; // Create, Update, Delete, Read, View
        public DateTime LastActivityTime { get; set; }
        public DateTime LoginTime { get; set; }

        public TimeSpan SessionTime { get; set;}
        public bool IsOnline { get; set; }
        public string? IpAddress { get; set; }
    }
}
