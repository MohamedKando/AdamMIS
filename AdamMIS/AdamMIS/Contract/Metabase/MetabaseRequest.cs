using System.ComponentModel.DataAnnotations;

namespace AdamMIS.Contract.Metabase
{
    public class MetabaseRequest
    {
        [Required]
        [Url]
        public string Url { get; set; } = string.Empty;

        [Required]
        [MinLength(1)]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}
