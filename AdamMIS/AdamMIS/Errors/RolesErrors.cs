namespace AdamMIS.Errors
{
    public class RolesErrors
    {
        public static readonly Error DuplicatedRole = new(
            "Role.DuplicatedRole",
            "There is another role with this name",
            StatusCodes.Status409Conflict);

        public static readonly Error PermissionNotFound = new(
            "Permission.NotFound",
            "One or more of the entered permissions do not exist",
            StatusCodes.Status404NotFound);

        public static readonly Error RoleNotFound = new(
            "Role.NotFound",
            "One or more of the entered roles do not exist",
            StatusCodes.Status404NotFound);

        public static readonly Error UserRoleExist = new(
            "Role.RoleExist",
            "The user already has this role/roles",
            StatusCodes.Status400BadRequest);



    }
}
