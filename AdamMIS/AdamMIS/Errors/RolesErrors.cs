namespace AdamMIS.Errors
{
    public class RolesErrors
    {
        public static readonly Error DublicatedRole = new Error("Role.DublicatedRole", "There is another role this this name");

        public static readonly Error PermissionNotFound = new Error("Permission.NotFound", " one or more of your entired permission is not exist");
    }
}
