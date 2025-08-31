using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdamMIS.Data.EntitiesConfigurations.UserEntitesConfigurations
{
    public class DepartmentConfigurations : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.HasData(
                  new Department { Id = 1, Name = "IT", HeadId = "03174B27-D47B-4C12-94AD-676B3BF14BC2" },//MohamedKandil
                  new Department { Id = 2, Name = "HR", HeadId = "9427f54e-4ca9-4662-b07b-ce078f19b4b9" },//Mohamed
                  new Department { Id = 3, Name = "Finance", HeadId = "f2cb2e80-c4d8-432d-95b5-09ae5ab13069" }, // Mohamed2
                  new Department { Id = 4, Name = "Operations", HeadId = "e43c1183-1abd-4b65-bc5a-64fa7b06d66e" },
                  new Department { Id = 5, Name = "CEO", HeadId = "9023369e-85f8-4389-85d0-d765caa0e1f9" }// Set this when you have Operations head
              );

        }
    }

}
