using AdamMIS.Contract.Roles;

namespace AdamMIS.Services.RolesServices
{
    public interface IRoleService
    {
        Task<IEnumerable<RolesResponse>> GetAllAsync(bool? includeDisabled = false);
        Task<RolesDetailsReponse> GetRolesDetailsAsync(string roleId);
        Task<Result<RolesDetailsReponse>> AddRoleAsync(RoleRequest request);

        Task<bool> UpdateRoleAsync(string id, RoleRequest request);
        Task<bool> ToggleStatusAsync(string id);
    }
}
