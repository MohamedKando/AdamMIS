using Microsoft.AspNetCore.Identity;

namespace AdamMIS.Entities
{
    public class ApplicationRole : IdentityRole
    {
        public bool IsDeleted { get; set; } = false;
        public bool IsDeafult { get; set; }

        public ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
    }
}
