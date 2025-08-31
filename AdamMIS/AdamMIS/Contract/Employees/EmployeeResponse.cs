namespace AdamMIS.Contract.Employees
{
    public class EmployeeResponse
    {
        public Guid Id { get; set; }

        // Basic Info
        public string EmployeeNumber { get; set; } = string.Empty;
        public string NameArabic { get; set; } = string.Empty;
        public string NameEnglish { get; set; } = string.Empty;
        public string PersonalEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string PayrollNumber { get; set; } = string.Empty;

        // Department
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;

        // Employee Type
        public bool IsMedical { get; set; }

        // Medical Fields
        public string? Qualification { get; set; }
        public string? Specialty { get; set; }
        public string? MedicalServiceCode { get; set; }
        public string? DoctorStatus { get; set; }
        public string? SeniorDoctorName { get; set; }
        public string? MedicalProfileType { get; set; }

        // System Permissions
        public string? SystemPermissions { get; set; }

        // IT Access
        public bool? InternetAccess { get; set; }
        public bool? ExternalEmail { get; set; }
        public bool? InternalEmail { get; set; }
        public string? FilesSharing { get; set; }
        public string? NetworkId { get; set; }
        public string? EmailId { get; set; }

        // CEO
        public string? CEOSignature { get; set; }

        // Workflow Status
        public string CurrentStep { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        // Step Completion Tracking (replaces approval fields)
        public DateTime? HRCompletedAt { get; set; }
        public string? HRCompletedBy { get; set; }
        public DateTime? DepartmentCompletedAt { get; set; }
        public string? DepartmentCompletedBy { get; set; }
        public DateTime? ITCompletedAt { get; set; }
        public string? ITCompletedBy { get; set; }
        public DateTime? CEOCompletedAt { get; set; }
        public string? CEOCompletedBy { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }
}
