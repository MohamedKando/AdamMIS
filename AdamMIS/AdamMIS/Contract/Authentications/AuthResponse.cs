namespace AdamMIS.Contract.Authentications
{
    public class AuthResponse
    {
        public string Id { get; set; } = string.Empty;
        //public string? Email { get; set; }

        public string UserName { get; set; } = string.Empty;
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        public string Token { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }


    }
}
