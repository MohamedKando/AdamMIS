using AdamMIS.Entities.ReportsEnitites;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdamMIS.Data.EntitiesConfigurations.ReportEntitiesConfugrations
{
    public class ReportConfigurations : IEntityTypeConfiguration<Reports>
    {
        public void Configure(EntityTypeBuilder<Reports> builder)
        {
            builder.ToTable("Reports");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.FileName)
                .IsRequired()
                .HasMaxLength(255);
            builder.Property(x => x.FilePath)
           .IsRequired()
           .HasMaxLength(500);

            builder.Property(x => x.CreatedAt)
           .IsRequired()
           .HasDefaultValueSql("GETDATE()");

            builder.Property(x => x.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

            builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
            builder.Property(x => x.CategoryId)
            .IsRequired();

            //builder.HasIndex(x => x.FileName);
            //builder.HasIndex(x => x.CategoryId);
            //builder.HasIndex(x => x.CreatedAt);
            //builder.HasIndex(x => x.IsActive);

            builder.HasOne(x => x.Category)
            .WithMany(x => x.Reports)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        
            // One-to-Many relationship with UserReports
            builder.HasMany(x => x.UserReports)
                .WithOne(x => x.Report)
                .HasForeignKey(x => x.ReportId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
