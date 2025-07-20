namespace AdamMIS.Services.UsersServices
{
    public interface IUserService
    {
        Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
    }
}
