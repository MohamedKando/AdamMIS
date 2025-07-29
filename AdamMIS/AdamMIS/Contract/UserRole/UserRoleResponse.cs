namespace AdamMIS.Contract.UserRole
{
    public class UserRoleResponse
    {
        public string UserId { get; set; } = string.Empty;
        //public string UserName { get; set; } = string.Empty;
        public string RoleId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string AssignedBy { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; }
    }
}
