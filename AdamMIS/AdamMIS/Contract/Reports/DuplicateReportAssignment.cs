namespace AdamMIS.Contract.Reports
{
    public class DuplicateReportAssignment
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int ReportId { get; set; }
        public string ReportFileName { get; set; }
        public string CategoryName { get; set; }
        public DateTime ExistingAssignedAt { get; set; }
        public string ExistingAssignedBy { get; set; }
    }
}
