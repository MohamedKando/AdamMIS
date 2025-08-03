namespace AdamMIS.Contract.Users
{
    public class CreateUserRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? DepartmentName { get; set; }
        public IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();
    }
}
