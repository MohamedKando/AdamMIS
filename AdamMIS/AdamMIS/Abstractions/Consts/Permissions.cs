namespace AdamMIS.Abstractions.Consts
{
    public static class Permissions
    {
        public static string Type { get; } = "permissions";

        //User Permissions
        public const string RegisterUsers = "auth:add";
        public const string DeleteUsers = "user:delete";
        public const string UpdateUsers = "user:update";
        public const string ReadUsers = "user:read";


        //Report Permissions
        public const string ReadReports = "report:read";
        public const string AddReports = "report:add";
        public const string UpdateReports = "report:update";
        public const string DeleteReports = "report:delete";
        
        
        //Category Permissions
        public const string ReadCategories = "category:read";
        public const string AddCategories = "category:add";
        public const string UpdateCategories = "category:update";
        public const string DeleteCategories = "category:delete";

        //Roles Permissions
        public const string ReadRoles = "roles:read";
        public const string AddRoles = "roles:add";
        public const string UpdateRoles = "Roles:update";
        public const string DeleteRoles = "Roles:delete";

        public const string Result = "results:read";


        //View Permisstions
        public const string ViewReportManager = "view:reportmanager";

        public static IList<string?> GetAllPermissions()
        {

            return typeof(Permissions).GetFields().Select(field =>field.GetValue(field) as string).ToList();
        }
    }
}
