


namespace AdamMIS.Errors
{
    public static class UserErrors
    {
        public static readonly Error UserInvalidCredentials = new(
            "User.InvalidCredentials",
            "Invalid Email / Password",
            StatusCodes.Status401Unauthorized);

        public static readonly Error UserNotFound = new(
            "User.NotFound",
            "There is no user with the given id",
            StatusCodes.Status404NotFound);

        public static readonly Error DublicatedUser = new(
            "User.DuplicatedUser",
            "There is a user with this name, please use another name",
            StatusCodes.Status409Conflict);

        public static readonly Error UserDisabled = new(
            "User.Disabled",
            "You have been banned, please contact the system admin",
            StatusCodes.Status403Forbidden);


        public static readonly Error ConfirmPasswordError = new("User.ConfirmPasswordError", "Confirm password does not match the new password.", StatusCodes.Status400BadRequest);
    }
}
