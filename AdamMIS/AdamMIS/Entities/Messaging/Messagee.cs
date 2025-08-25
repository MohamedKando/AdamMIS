using System.ComponentModel.DataAnnotations;

namespace AdamMIS.Entities.Messaging
{
    public class Messagee
    {
        public int Id { get; set; }

        [Required]
        public string SenderId { get; set; } // Assuming you're using Identity with string IDs

        [Required]
        public string RecipientId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;

        public DateTime? ReadAt { get; set; }
    }
}
