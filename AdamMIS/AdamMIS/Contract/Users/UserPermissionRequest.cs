namespace AdamMIS.Contract.Users
{
    public class UserPermissionRequest
    {
        public string UserId { get; set; } = default!;
        public List<string> Permissions { get; set; } = new();
    }
    }
