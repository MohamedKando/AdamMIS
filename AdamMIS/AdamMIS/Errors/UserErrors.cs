


namespace AdamMIS.Errors
{
    public static class UserErrors
    {
        public static readonly Error UserInvalidCredentials = new Error("User.InvalidCredentials", "Invalid Email / Password");

        public static readonly Error UserNotFound = new Error("User.NotFound", "There is no user with the give id");

        public static readonly Error DublicatedUser = new Error("User.DublicatedUser", "There is User with this  name please user another name");
    }
}
