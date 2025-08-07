namespace AdamMIS.Contract.Users
{
    public class UserResponse
    {
        public string Id { get; set; } = string.Empty;
        public string UserName {  get; set; } =string.Empty;
        public bool IsDisabled { get; set; }
        public string? Title { get; set; }
        public string? DepartmentName { get; set; }
        public string? InternalPhone {  get; set; }
        public string? UserPhone { get; set; }
        public string? PhotoPath { get; set; }
        public IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();
    }
}
