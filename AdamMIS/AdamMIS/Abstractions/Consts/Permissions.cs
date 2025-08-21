namespace AdamMIS.Abstractions.Consts
{
    public static class Permissions
    {
        public static string Type { get; } = "permissions";

        //User Permissions
        public const string RegisterUsers = "Register Users";
        public const string DeleteUsers = "Delete Users";
        public const string UpdateUsers = "Update Users";
        public const string ReadUsers = "Read Users";


        //Report Permissions
        public const string ReadReports = "Read Reports";
        public const string AddReports = "Add Reports";
        public const string UpdateReports = "Update Reports";
        public const string DeleteReports = "Delete Reports";
        
        
        //Category Permissions
        public const string ReadCategories = "Read Categories";
        public const string AddCategories = "Add Categories";
        public const string UpdateCategories = "Update Categories";
        public const string DeleteCategories = "Delete Categories";

        //Roles Permissions
        public const string ReadRoles = "Read Roles";
        public const string AddRoles = "Add Roles";
        public const string UpdateRoles = "Update Roles";
        public const string DeleteRoles = "Delete Roles";


        //logs
        public const string DeleteLogs = "Delete Logs";






        public const string Result = "Read Result";


        //View Permisstions
        public const string ViewReportManager = "View Report Manager";

        public const string ViewAdminManager = "View Admin Manager";

        public const string AuditsLogs = "View System Logs";
        public const string ActivityLogs = "View Activity Logs";



        //indevdual permissions
        public const string ViewMRM = "View MRM";
        public const string ViewFDMS = "View F DMS";
        public const string ViewHDMS = "View H DMS";
        public const string ViewAIMS = "View AIMS";





        // Get All permission to display in frontend
        public static IList<string?> GetAllPermissions()
        {
            return typeof(Permissions)
                .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
                .Select(fi => fi.GetRawConstantValue() as string)
                .Where(value => value != Permissions.DeleteLogs) 
                .ToList();
        }


        public static IList<string?> GetIndividualPermissions()
        {
            var individualPermissions = new[]
            {
        ViewMRM,
        ViewFDMS,
        ViewHDMS,
        ViewAIMS
    };

            return individualPermissions.ToList()!;
        }
    }
}
