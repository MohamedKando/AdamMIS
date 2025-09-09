

namespace AdamMIS.Services.ReportsServices
{

        public interface IReportService
        {
            // Category Management
            Task<RCategoryResponse> CreateCategoryAsync(RCategoryRequest request);
            Task<RCategoryResponse?> GetCategoryByIdAsync(int id);
            Task<RCategoryResponse> UpdateCategoryAsync(int id, RCategoryRequest request);
            Task<Result> DeleteCategoryAsync(int id);
            Task<IEnumerable<RCategoryResponse>> GetAllCategoriesAsync();



            // Report Management
            Task<ReportResponse?> GetReportByIdAsync(int id);
            Task<IEnumerable<ReportResponse>> GetAllReportsAsync();
            Task<IEnumerable<ReportResponse>> GetReportsByCategoryAsync(int categoryId);
            Task<ReportResponse> UploadReportAsync(ReportRequest request, string createdBy);          
            Task<bool> DeleteReportAsync(int id);





        // User Report Assignment
            Task<ReportAssignmentResult> AssignReportsToUsersAsync(UserReportRequest request, string assignedBy);
            Task<bool> RemoveUserReportAssignmentAsync(int userReportId);
            Task<IEnumerable<UserReportResponse>> GetUserReportsAsync(string userId);
            Task<IEnumerable<UserReportResponse>> GetReportUsersAsync(int reportId);
            Task<IEnumerable<UserReportResponse>> GetAllUserReportsAsync();



        //// File Management
        //Task<byte[]?> GetReportFileAsync(int reportId);
        //Task<string?> GetReportFilePathAsync(int reportId);
        Task<Result> ClearAllReportsAsync(CancellationToken cancellationToken);





        }
    
}
