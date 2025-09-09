namespace AdamMIS.Contract.Reports
{
    public class ReportAssignmentResult
    {
        public List<UserReportResponse> NewAssignments { get; set; } = new List<UserReportResponse>();
        public List<DuplicateReportAssignment> DuplicateAssignments { get; set; } = new List<DuplicateReportAssignment>();
        public int TotalRequested { get; set; }
        public int SuccessfulAssignments { get; set; }
        public int DuplicateCount { get; set; }
    }
}
