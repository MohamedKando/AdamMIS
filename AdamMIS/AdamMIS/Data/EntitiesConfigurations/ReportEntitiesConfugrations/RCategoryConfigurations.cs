using AdamMIS.Entities.ReportsEnitites;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdamMIS.Data.EntitiesConfigurations.ReportEntitiesConfugrations
{
    public class RCategoryConfigurations : IEntityTypeConfiguration<RCategories>
    {
        public void Configure(EntityTypeBuilder<RCategories> builder)
        {
            builder.ToTable("RCategories");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.HasIndex(x => x.Name)
            .IsUnique();

            // One-to-Many relationship with Reports
            builder.HasMany(x => x.Reports)
                .WithOne(x => x.Category)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
