namespace AdamMIS.Contract.Employees
{
    public class EmployeeRoleRequest
    {
      
            public List<string> Roles { get; set; } = new List<string>();
            public int? DepartmentId { get; set; }
            public string DepartmentName { get; set; }
     
    }
}
