using System.ComponentModel.DataAnnotations;

namespace AdamMIS.Contract.Reports
{
    public class RCategoryRequest
    {
       
            [Required]
            [StringLength(100, MinimumLength = 2)]
            public string Name { get; set; } = string.Empty;


    }
}
