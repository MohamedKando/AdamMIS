using Microsoft.AspNetCore.Authorization;

namespace AdamMIS.Authentications.Filters
{
    public class HasPermissionAttribute : AuthorizeAttribute
    {

        public HasPermissionAttribute(string permission)
        {
            Policy = permission;
        }
    }

}
