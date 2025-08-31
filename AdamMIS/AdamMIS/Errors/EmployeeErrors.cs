namespace AdamMIS.Errors
{
    public class EmployeeErrors
    {
        public static readonly Error UserIsNotHr = new(
           "User.UnAuthrithed",
           "Only Hr can edit this section",
           StatusCodes.Status401Unauthorized);

        public static readonly Error UserIsNotDepartmentHead = new(
   "User.UnAuthrithed",
   "Only DepartmentHead can edit this section",
   StatusCodes.Status401Unauthorized);

        public static readonly Error UserIsNotITtHead = new(
   "User.UnAuthrithed",
   "Only IT Head can edit this section",
   StatusCodes.Status401Unauthorized);
        public static readonly Error UserIsNotCEO = new(
"User.UnAuthrithed",
"Only CEO can edit this section",
StatusCodes.Status401Unauthorized);

        public static readonly Error EmployeeNotFound = new(
             "Employee.NotFound",
             "There Is No Employee With that id",
                StatusCodes.Status404NotFound);
        public static readonly Error DublicatedEmployee = new(
        "Employee.Dublicated",
        "There is already employee with that number",
        StatusCodes.Status409Conflict);



    }
}
