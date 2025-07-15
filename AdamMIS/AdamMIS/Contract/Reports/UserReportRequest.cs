using System.ComponentModel.DataAnnotations;

namespace AdamMIS.Contract.Reports
{
    public class UserReportRequest
    {
        [Required]
        [MinLength(1)]
        public List<int> ReportIds { get; set; } = new();
        [Required]
        [MinLength(1)]
        public List<string> UserIds { get; set; } = new();
    }
}
