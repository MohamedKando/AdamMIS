using AdamMIS.Entities.ReportsEnitites;
using Microsoft.AspNetCore.Identity;

namespace AdamMIS.Entities
{
    public class ApplicationUser : IdentityUser
    {

        public bool IsDisabled { get; set; }
        public ICollection<UserReports> UserReports { get; set; } = new List<UserReports>();
        public ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
    }
}
