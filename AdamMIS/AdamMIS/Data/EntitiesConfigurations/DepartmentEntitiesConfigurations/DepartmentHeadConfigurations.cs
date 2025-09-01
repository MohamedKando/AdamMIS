using AdamMIS.Entities.DepartmentEntities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdamMIS.Data.EntitiesConfigurations.DepartmentEntitiesConfigurations
{
    public class DepartmentHeadConfigurations : IEntityTypeConfiguration<DepartmentHead>
    {
        public void Configure(EntityTypeBuilder<DepartmentHead> builder)
        {
            builder.HasData(
                new DepartmentHead { Id = 1, DepartmentId = 1, HeadId = "03174B27-D47B-4C12-94AD-676B3BF14BC2" }, // IT
                new DepartmentHead { Id = 2, DepartmentId = 2, HeadId = "9427f54e-4ca9-4662-b07b-ce078f19b4b9" }, // HR
                new DepartmentHead { Id = 3, DepartmentId = 3, HeadId = "f2cb2e80-c4d8-432d-95b5-09ae5ab13069" }, // Finance
                new DepartmentHead { Id = 4, DepartmentId = 4, HeadId = "e43c1183-1abd-4b65-bc5a-64fa7b06d66e" }, // Operations
                new DepartmentHead { Id = 5, DepartmentId = 5, HeadId = "9023369e-85f8-4389-85d0-d765caa0e1f9" }  // CEO
            );
        }
    }
}
