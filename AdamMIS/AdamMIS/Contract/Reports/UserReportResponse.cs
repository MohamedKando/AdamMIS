namespace AdamMIS.Contract.Reports
{
    public class UserReportResponse
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
       
        public int ReportId { get; set; }
        public string ReportFileName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; }
        public string AssignedBy { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
