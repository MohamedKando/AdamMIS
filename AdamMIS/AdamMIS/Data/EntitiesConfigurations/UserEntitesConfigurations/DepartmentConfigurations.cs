using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdamMIS.Data.EntitiesConfigurations.UserEntitesConfigurations
{
    public class DepartmentConfigurations : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.HasData(
                new Department { Id = 1, Name = "IT" },
                new Department { Id = 2, Name = "HR" },
                new Department { Id = 3, Name = "Finance" },
                new Department { Id = 4, Name = "Operations" }
            );
        }
    }

}
