using AdamMIS.Entities.ReportsEnitites;
using Microsoft.AspNetCore.Identity;

namespace AdamMIS.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public ICollection<UserReports> UserReports { get; set; } = new List<UserReports>();
    }
}
