using AdamMIS.Contract.Metabase;

namespace AdamMIS.Services.MetabaseServices
{
    public interface IMetabaseServices
    {
        Task<IEnumerable<MetabaseResponse>> GetAllUrlsAsync();
        Task<MetabaseResponse?> GetUrlByIdAsync(int id);
        Task<MetabaseResponse> CreateUrlAsync(MetabaseRequest request, string createdBy);
        Task<MetabaseResponse?> UpdateUrlAsync(int id,MetabaseRequest request);
        Task<bool> DeleteUrlAsync(int id);

        Task<IEnumerable<UserMetabaseResponse>> AssignUrlsToUsersAsync(UserMetabaseRequest request, string assignedBy);
        Task<bool> UnassignUrlsFromUsersAsync(int userMetabaseId);


        Task<IEnumerable<UserMetabaseResponse>> GetUrlAssignmentsAsync(int metabaseId);
        Task<UserMetabaseResponse?> GetUserAssignedUrlsAsync(string userId);
        Task<IEnumerable<UserMetabaseResponse>> GetAllUsersUrlsAsync();
        Task<IEnumerable<UserMetabaseResponse>> GetAllAssignmentsAsync();
    }
}

