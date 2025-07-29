namespace AdamMIS.Contract.UserRole
{
    public class UserRoleRequest
    {

        public string UserId { get; set; } = string.Empty;
        public IEnumerable<string> RoleIds { get; set; } = Enumerable.Empty<string>();
    }
}
