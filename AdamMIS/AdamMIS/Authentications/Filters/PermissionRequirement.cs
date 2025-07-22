using Microsoft.AspNetCore.Authorization;

namespace AdamMIS.Authentications.Filters
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }

        public string Permission { get; }

    }
}
