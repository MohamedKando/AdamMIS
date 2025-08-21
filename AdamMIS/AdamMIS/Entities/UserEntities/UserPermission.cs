using AdamMIS.Entities.UserEntities;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AdamMIS.Entities
{
    public class UserPermission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = default!;

        [Required]
        [MaxLength(100)]
        public string Permission { get; set; } = default!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual ApplicationUser User { get; set; } = default!;
    }
}