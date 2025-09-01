using AdamMIS.Entities.DepartmentEntities;
using AdamMIS.Entities.MetaBase;
using AdamMIS.Entities.ReportsEnitites;
using Microsoft.AspNetCore.Identity;

namespace AdamMIS.Entities.UserEntities
{
    public class ApplicationUser : IdentityUser
    {

        public bool IsDisabled { get; set; }

        public string? Title { get; set; } // New field
        public int? DepartmentId { get; set; } // FK
        public string? PhotoPath { get; set; }
       

        public string? InternalPhone { get; set; }
        public ICollection<UserReports> UserReports { get; set; } = new List<UserReports>();
        public Department? Department { get; set; } // Navigation property
        public ICollection<UsersMetabases> UserMetabase { get; set; } = new List<UsersMetabases>();
        public ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
    }
}
