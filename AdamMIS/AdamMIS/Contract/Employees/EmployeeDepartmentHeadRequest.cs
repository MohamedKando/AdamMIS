namespace AdamMIS.Contract.Employees
{
    public class EmployeeDepartmentHeadRequest
    {
        public Guid EmployeeId { get; set; }

        // Medical Fields (only if IsMedical = true)
        public string? Qualification { get; set; }
        public string? Specialty { get; set; }
        public string? MedicalServiceCode { get; set; }
        public string? DoctorStatus { get; set; }
        public string? SeniorDoctorName { get; set; }
        public string? MedicalProfileType { get; set; }

        // System Permissions
        public string SystemPermissions { get; set; } = string.Empty;
    }
}
