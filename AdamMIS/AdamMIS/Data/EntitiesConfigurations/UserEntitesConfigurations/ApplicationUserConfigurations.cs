using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdamMIS.Data.EntitiesConfigurations.UserEntitesConfigurations
{
    public class ApplicationUserConfigurations : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
          

          
            var passwordHasher = new PasswordHasher<ApplicationUser>();
            builder.HasData(new ApplicationUser
            {
                Id = DeafultUsers.AdminId,
                UserName = DeafultUsers.AdminName,
                SecurityStamp = DeafultUsers.AdminSecurityStamp,
                NormalizedUserName = DeafultUsers.AdminName.ToUpper(),
                ConcurrencyStamp = DeafultUsers.AdminConcurrencyStamp,
                EmailConfirmed=true,
                PasswordHash=passwordHasher.HashPassword(null!,DeafultUsers.AdminPassword)
            });

        }
    }
}
