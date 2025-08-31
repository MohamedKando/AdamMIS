namespace AdamMIS.Contract.Employees
{
    public class EmployeeHRRequest
    {
        public string EmployeeNumber { get; set; } = string.Empty;
        public string NameArabic { get; set; } = string.Empty;
        public string NameEnglish { get; set; } = string.Empty;
        public string PersonalEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string PayrollNumber { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public bool IsMedical { get; set; }
    }
}
