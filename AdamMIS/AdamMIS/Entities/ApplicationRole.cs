using Microsoft.AspNetCore.Identity;

namespace AdamMIS.Entities
{
    public class ApplicationRole : IdentityRole
    {
        public bool IsDeafult { get; set; }
    }
}
