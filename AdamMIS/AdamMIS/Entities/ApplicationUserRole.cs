namespace AdamMIS.Entities
{
    public class ApplicationUserRole
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string RoleId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string AssignedBy { get; set; } = string.Empty; // User ID who made the assignment
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public ApplicationUser User { get; set; } 
        public ApplicationRole Role { get; set; }
       
    }
}
