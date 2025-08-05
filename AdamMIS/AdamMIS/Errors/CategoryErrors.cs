namespace AdamMIS.Errors
{
    public class CategoryErrors
    {
        public static readonly Error CategoryNotFound = new Error("Category.NotFound", "There is no Category with that name", StatusCodes.Status404NotFound);
    }
}
