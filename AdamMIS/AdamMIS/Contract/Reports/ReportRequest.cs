using System.ComponentModel.DataAnnotations;

namespace AdamMIS.Contract.Reports
{
    public class ReportRequest
    {
        [Required]
        public IFormFile File { get; set; } = default!;

        [Required]
        public int CategoryId { get; set; }
    }
}
