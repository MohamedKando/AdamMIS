namespace AdamMIS.Contract.Roles
{
    public class RoleRequest
    {
        public string Name { get; set; }
        public IEnumerable<string> Permissions { get; set; } = Enumerable.Empty<string>();
    }
}
