using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdamMIS.Data.EntitiesConfigurations.UserEntitesConfigurations
{
    public class RoleConfigurations : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            builder.HasData([

                new ApplicationRole
                {
                    Id=DeafultRole.AdminRoleId,
                    Name=DeafultRole.SuperAdmin,
                    NormalizedName=DeafultRole.SuperAdmin.ToUpper(),
                    ConcurrencyStamp=DeafultRole.AdminRoleConcurrencyStamp
                },
                new ApplicationRole
                {
                    Id=DeafultRole.MemberRoleId,
                    Name=DeafultRole.Member,
                    NormalizedName=DeafultRole.Member.ToUpper(),
                    ConcurrencyStamp=DeafultRole.MemberRoleConcurrencyStamp,
                    IsDeafult=true
                    
                }
                ]);
        }
    }
}
