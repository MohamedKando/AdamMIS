using System.ComponentModel.DataAnnotations;

namespace AdamMIS.Authentications
{
    public class JwtOptions
    {
        [Required]
        public string Key { get; init; } = string.Empty;
        [Required]
        public string Issuer { get; init; } = string.Empty;
        [Required]
        public string Audience { get; init; } = string.Empty;
        [Required]
        [Range(0, int.MaxValue)]
        public int ExpiryMinutes { get; init; }
    }
}

