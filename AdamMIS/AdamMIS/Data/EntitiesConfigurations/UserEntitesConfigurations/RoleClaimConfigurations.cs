using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdamMIS.Data.EntitiesConfigurations.UserEntitesConfigurations
{
    public class RoleClaimConfigurations : IEntityTypeConfiguration<IdentityRoleClaim<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
        {
            var permissions = Permissions.GetAllPermissions();
            var adminClaims = new List<IdentityRoleClaim<string>>();

            for (var i = 0; i < permissions.Count; i++)
            {
                adminClaims.Add(new IdentityRoleClaim<string>
                {
                    Id = -(i + 1),
                    ClaimType = Permissions.Type,
                    ClaimValue = permissions[i],
                    RoleId=DeafultRole.AdminRoleId
                   

                });
            
            }
            builder.HasData(adminClaims);
        }
    }
}
