using AdamMIS.Entities.EmployeeEntities;
using AdamMIS.Entities.UserEntities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdamMIS.Data.EntitiesConfigurations.EmplyeeEntitiesConfigurations
{
    public class EmployeeConfigurations : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.EmployeeNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(e => e.EmployeeNumber)
                .IsUnique();

            builder.Property(e => e.NameArabic)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.NameEnglish)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.PersonalEmail)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(e => e.ContactPhone)
                .HasMaxLength(20);

            builder.Property(e => e.PayrollNumber)
                .HasMaxLength(50);

            builder.Property(e => e.CurrentStep)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("HR");

            builder.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Draft");

            // Medical fields - optional
            builder.Property(e => e.Qualification).HasMaxLength(100);
            builder.Property(e => e.Specialty).HasMaxLength(100);
            builder.Property(e => e.MedicalServiceCode).HasMaxLength(50);
            builder.Property(e => e.DoctorStatus).HasMaxLength(100);
            builder.Property(e => e.SeniorDoctorName).HasMaxLength(200);
            builder.Property(e => e.MedicalProfileType).HasMaxLength(100);

            // IT fields
            builder.Property(e => e.FilesSharing).HasMaxLength(20);
            builder.Property(e => e.NetworkId).HasMaxLength(100);
            builder.Property(e => e.EmailId).HasMaxLength(250);

            // Foreign Keys
            builder.HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);




        }
    }
  
}
