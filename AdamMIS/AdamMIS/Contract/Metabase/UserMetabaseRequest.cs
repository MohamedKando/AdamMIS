using System.ComponentModel.DataAnnotations;

namespace AdamMIS.Contract.Metabase
{
    public class UserMetabaseRequest
    {
        [Required]
        [MinLength(1)]
        public List<int> MetabaseIds { get; set; } = new();

        [Required]
        [MinLength(1)]
        public List<string> UserIds { get; set; } = new();
    }
}
