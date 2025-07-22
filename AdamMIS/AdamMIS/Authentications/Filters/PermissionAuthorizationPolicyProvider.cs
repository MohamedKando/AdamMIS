using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace AdamMIS.Authentications.Filters
{
    public class PermissionAuthorizationPolicyProvider: DefaultAuthorizationPolicyProvider
    {
        private readonly AuthorizationOptions _authorizationOptions;
        public PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
         _authorizationOptions = options.Value;   
        }

        public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            var policy = await base.GetPolicyAsync(policyName);
            if (policy != null)
            {
                return policy;
            }

            // Dynamically create and register a new permission-based policy
            var permissionPolicy = new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(policyName))
                .Build();

            // Optionally register it in the in-memory policy cache
            _authorizationOptions.AddPolicy(policyName, permissionPolicy);

            return permissionPolicy;
        }
    }
}
