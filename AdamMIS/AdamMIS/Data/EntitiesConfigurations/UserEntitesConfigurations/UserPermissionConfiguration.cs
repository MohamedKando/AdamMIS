using AdamMIS.Entities.UserEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdamMIS.Data.EntitiesConfigurations.UserEntitesConfigurations
{
    public class UserPermissionConfiguration : IEntityTypeConfiguration<UserPermission>
    {
        public void Configure(EntityTypeBuilder<UserPermission> builder)
        {
            // Primary Key
            builder.HasKey(e => e.Id);

            // Unique Index on UserId + Permission
            builder.HasIndex(e => new { e.UserId, e.Permission })
                   .IsUnique()
                   .HasDatabaseName("IX_UserPermissions_UserId_Permission");

            // Relationship with User
            builder.HasOne(e => e.User)
                   .WithMany() // or .WithMany(u => u.UserPermissions) if you have a collection
                   .HasForeignKey(e => e.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
