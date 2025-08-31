namespace AdamMIS.Contract.Employees
{
    public class EmployeeITRequest
    {
        public Guid EmployeeId { get; set; }

        public bool InternetAccess { get; set; }
        public bool ExternalEmail { get; set; }
        public bool InternalEmail { get; set; }
        public string FilesSharing { get; set; } = string.Empty; // "None", "ReadOnly", "FullControl"
        public string? NetworkId { get; set; }
        public string? EmailId { get; set; }
    }
}
