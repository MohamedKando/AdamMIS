using AdamMIS.Entities.EmployeeEntities;
using AdamMIS.Entities.UserEntities;

namespace AdamMIS.Entities.DepartmentEntities
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<DepartmentHead> Heads { get; set; } = new List<DepartmentHead>();
        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
