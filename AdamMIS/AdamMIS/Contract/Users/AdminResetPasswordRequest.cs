namespace AdamMIS.Contract.Users
{
    public class AdminResetPasswordRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
