using AdamMIS.Entities.EmployeeEntities;
using AdamMIS.Entities.UserEntities;

namespace AdamMIS.Entities
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? HeadId { get; set; } // Just the FK, no navigation needed
        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
