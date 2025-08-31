

namespace AdamMIS.Services.EmployeeServices
{
    public interface IEmployeeService
    {
        // ============= HR STEP =============
        Task<Result<EmployeeResponse>> CreateEmployeeAsync(EmployeeHRRequest request, string createdBy);
        Task<Result<EmployeeResponse>> CompleteHRStepAsync(Guid employeeId, string userId);

        // ============= DEPARTMENT HEAD STEP =============
        Task<Result<EmployeeResponse>> UpdateDepartmentInfoAsync(EmployeeDepartmentHeadRequest request, string userId);
        Task<Result<EmployeeResponse>> CompleteDepartmentStepAsync(Guid employeeId, string userId);

        // ============= IT STEP =============
        Task<Result<EmployeeResponse>> UpdateITInfoAsync(EmployeeITRequest request, string userId);
        Task<Result<EmployeeResponse>> CompleteITStepAsync(Guid employeeId, string userId);

        // ============= CEO STEP =============
        Task<Result<EmployeeResponse>> UpdateCEOInfoAsync(EmployeeCEORequest request, string userId);
        Task<Result<EmployeeResponse>> CompleteCEOStepAsync(Guid employeeId, string userId);

        // ============= QUERY METHODS =============
        Task<EmployeeResponse> GetEmployeeByIdAsync(Guid id);
        Task<IEnumerable<EmployeeResponse>> GetAllEmployeesAsync();
        Task<IEnumerable<EmployeeResponse>> GetMyPendingApprovalsAsync(string userId);
        Task<IEnumerable<EmployeeResponse>> GetEmployeesByStatusAsync(string status);
        Task<IEnumerable<EmployeeResponse>> GetEmployeesByStepAsync(string step);
        Task<IEnumerable<EmployeeResponse>> GetEmployeesByDepartmentAsync(int departmentId);
        Task<IEnumerable<EmployeeResponse>> GetCompletedEmployeesAsync(DateTime? fromDate = null, DateTime? toDate = null);

        // ============= ACCESS CONTROL =============
        Task<bool> CanUserAccessEmployeeAsync(string userId, Guid employeeId);
        Task<EmployeeRoleRequest> GetUserRoleInfoAsync(string userId);
        Task<bool> CanUserEditEmployeeStepAsync(string userId, Guid employeeId, string step);
        Task<string> GetUserRoleForEmployeeAsync(string userId, Guid employeeId);
        Task<bool> IsUserAuthorizedForStepAsync(string userId, Guid employeeId, string step);
        Task<List<Employee>> GetEmployeesByStepForUserAsync(string stepName, string userId);

        // ============= STATISTICS =============
        Task<Dictionary<string, int>> GetWorkflowStatisticsAsync();
        Task<string> GetUserPrimaryRoleAsync(string userId);
        Task<Dictionary<string, int>> GetDepartmentStatisticsAsync();
    }
}