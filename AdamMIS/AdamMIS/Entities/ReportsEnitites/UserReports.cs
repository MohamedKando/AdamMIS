namespace AdamMIS.Entities.ReportsEnitites
{
    public class UserReports
    {
        public int Id { get; set; }
        
        public string UserId { get; set; } = string.Empty;
        
        public int ReportId { get; set; }

        public DateTime AssignedAt { get; set; }
        public string AssignedBy { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = default!;
        public Reports Report { get; set; } = default!;
    }
}
