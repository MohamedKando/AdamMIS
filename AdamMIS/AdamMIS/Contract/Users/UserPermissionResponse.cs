namespace AdamMIS.Contract.Users
{
    public class UserPermissionResponse
    {
        public string UserId { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public List<string> IndividualPermissions { get; set; } = new();
        public List<string> RoleBasedPermissions { get; set; } = new();
        public List<string> AllPermissions { get; set; } = new();
    }
}
