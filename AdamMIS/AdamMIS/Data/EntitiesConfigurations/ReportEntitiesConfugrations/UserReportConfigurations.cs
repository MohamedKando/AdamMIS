using AdamMIS.Entities.ReportsEnitites;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdamMIS.Data.EntitiesConfigurations.ReportEntitiesConfugrations
{
    public class UserReportsConfigurations : IEntityTypeConfiguration<UserReports>
    {
        public void Configure(EntityTypeBuilder<UserReports> builder)
        {
            builder.ToTable("UserReports");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.UserId)
                .IsRequired()
                .HasMaxLength(450); // Standard length for AspNetCore Identity UserId

            builder.Property(x => x.ReportId)
                .IsRequired();

            builder.Property(x => x.AssignedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()"); // SQL Server

            builder.Property(x => x.AssignedBy)
                .IsRequired()
                .HasMaxLength(100);



            //// Composite unique index to prevent duplicate assignments
            //builder.HasIndex(x => new { x.UserId, x.ReportId })
            //    .IsUnique()
            //    .HasDatabaseName("IX_UserReports_UserId_ReportId_Unique");

            //// Additional indexes for performance
            //builder.HasIndex(x => x.UserId)
            //    .HasDatabaseName("IX_UserReports_UserId");

            //builder.HasIndex(x => x.ReportId)
            //    .HasDatabaseName("IX_UserReports_ReportId");

            //builder.HasIndex(x => x.AssignedAt)
            //    .HasDatabaseName("IX_UserReports_AssignedAt");



            // Many-to-One relationship with User
            builder.HasOne(x => x.User)
                .WithMany(x => x.UserReports)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade); // When user is deleted, remove assignments

            // Many-to-One relationship with Report
            builder.HasOne(x => x.Report)
                .WithMany(x => x.UserReports)
                .HasForeignKey(x => x.ReportId)
                .OnDelete(DeleteBehavior.Cascade); // When report is deleted, remove assignments
        }
    }
}
