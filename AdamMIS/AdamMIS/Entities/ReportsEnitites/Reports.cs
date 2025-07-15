namespace AdamMIS.Entities.ReportsEnitites
{
    public class Reports
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public RCategories Category { get; set; } = default!;
        public int CategoryId { get; set; }
        public DateTime CreatedAt { get; set; }

        public string CreatedBy { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true; // Added - for soft delete.
        public ICollection<UserReports> UserReports { get; set; } = new List<UserReports>();
    }
}
