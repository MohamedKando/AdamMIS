using AdamMIS.Entities.UserEntities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdamMIS.Data.EntitiesConfigurations.UserEntitesConfigurations
{
    public class UserRoleConfigurations : IEntityTypeConfiguration<ApplicationUserRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserRole> builder)
        {
            builder.HasOne(ur => ur.User)
                .WithMany()
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ur => ur.Role)
                .WithMany()
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
