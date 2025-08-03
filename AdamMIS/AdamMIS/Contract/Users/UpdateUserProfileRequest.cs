namespace AdamMIS.Contract.Users
{
    public class UpdateUserProfileRequest
    {
        public string? UserName { get; set; }
        public string? Title { get; set; }
        public string? Department{ get; set; }
        public string? Email { get; set; }
    }
}
