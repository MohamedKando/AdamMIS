namespace AdamMIS.Abstractions.Consts
{
    public static class Permissions
    {
        public static string Type { get; } = "permissions";


        public const string RegisterUsers = "auth:add";
        public const string DeleteUsers = "user:delete";
        public const string UpdateUsers = "user:update";
        public const string ReadUsers = "user:read";

        public const string ReadReports = "report:read";
        public const string AddReports = "report:add";
        public const string UpdateReports = "report:update";
        public const string DeleteReports = "report:delete";

        public const string ReadCategories = "category:read";
        public const string AddCategories = "category:add";
        public const string UpdateCategories = "category:update";
        public const string DeleteCategories = "category:delete";

        public const string ReadRoles = "roles:read";
        public const string AddRoles = "roles:add";
        public const string UpdateRoles = "Roles:update";
        public const string DeleteRoles = "Roles:delete";

        public const string Result = "results:read";

        public static IList<string?> GetAllPermissions()
        {

            return typeof(Permissions).GetFields().Select(field =>field.GetValue(field) as string).ToList();
        }
    }
}
