using AdamMIS.Entities.ReportsEnitites;
using Microsoft.AspNetCore.Identity;

namespace AdamMIS.Entities
{
    public class ApplicationUser : IdentityUser
    {

        public bool IsDisabled { get; set; }

        public string? Title { get; set; } // New field
        public int? DepartmentId { get; set; } // FK

        public Department? Department { get; set; } // Navigation property
        public ICollection<UserReports> UserReports { get; set; } = new List<UserReports>();
        public ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
    }
}
