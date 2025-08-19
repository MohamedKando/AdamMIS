namespace AdamMIS.Errors
{
    public static class NetworkErrors
    {
        public static readonly Error NetworkNotFound = new(
           "NetWorks.NotFound",
           "There is issue in Netword path",
           StatusCodes.Status400BadRequest);
    }
}
