namespace AdamMIS.Errors
{
    public static class DepartmentErrors
    {
        public static readonly Error DepartmentNotFound = new Error("Department.NotFound", "There is no Department with that name", StatusCodes.Status404NotFound);
    }
}
