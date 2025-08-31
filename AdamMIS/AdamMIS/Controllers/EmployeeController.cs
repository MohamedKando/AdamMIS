using AdamMIS.Authentications;
using AdamMIS.Contract.Departments;
using AdamMIS.Contract.Employees;
using AdamMIS.Entities.EmployeeEntities;
using AdamMIS.Services.EmployeeServices;
using AdamMIS.Services.UsersServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdamMIS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
   // [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IUserService _userService;

        public EmployeesController(IEmployeeService employeeService , IUserService userService )
        {
            _employeeService = employeeService;
            _userService = userService;
        }

        // ============= HR STEP ENDPOINTS =============
        [HttpPost("hr/create")]
        public async Task<ActionResult<EmployeeResponse>> CreateEmployee([FromBody] EmployeeHRRequest request)
        {
            
                var userId = GetCurrentUserId();
                var result = await _employeeService.CreateEmployeeAsync(request, userId);
                if (result.IsFailure)
                    return Problem(result.Error.Description, result.Error.Code, result.Error.StatusCode);
                return Ok(result);
            

        }

        [HttpPost("hr/{employeeId}/complete")]
        public async Task<ActionResult<EmployeeResponse>> CompleteHRStep(Guid employeeId)
        {
          
                var userId = GetCurrentUserId();
                var result = await _employeeService.CompleteHRStepAsync(employeeId, userId);
            if (result.IsFailure)
                return Problem(result.Error.Description, result.Error.Code, result.Error.StatusCode);
            return Ok(result);


        }

        // ============= DEPARTMENT HEAD STEP ENDPOINTS =============
        [HttpPut("department/update")]
        public async Task<ActionResult<EmployeeResponse>> UpdateDepartmentInfo([FromBody] EmployeeDepartmentHeadRequest request)
        {
            
                var userId = GetCurrentUserId();
                var result = await _employeeService.UpdateDepartmentInfoAsync(request, userId);
                if (result.IsFailure)
                    return Problem(result.Error.Description, result.Error.Code, result.Error.StatusCode);
                return Ok(result);
            
        }

        [HttpPost("department/{employeeId}/complete")]
        public async Task<ActionResult<EmployeeResponse>> CompleteDepartmentStep(Guid employeeId)
        {
           
                var userId = GetCurrentUserId();
                var result = await _employeeService.CompleteDepartmentStepAsync(employeeId, userId);
                if (result.IsFailure)
                    return Problem(result.Error.Description, result.Error.Code, result.Error.StatusCode);
                return Ok(result);
            }

        // ============= IT STEP ENDPOINTS =============
        [HttpPut("it/update")]
        public async Task<ActionResult<EmployeeResponse>> UpdateITInfo([FromBody] EmployeeITRequest request)
        {
             var userId = GetCurrentUserId();
                var result = await _employeeService.UpdateITInfoAsync(request, userId);
                if (result.IsFailure)
                    return Problem(result.Error.Description, result.Error.Code, result.Error.StatusCode);
                return Ok(result);
            }

        [HttpPost("it/{employeeId}/complete")]
        public async Task<ActionResult<EmployeeResponse>> CompleteITStep(Guid employeeId)
        {
          
                var userId = GetCurrentUserId();
                var result = await _employeeService.CompleteITStepAsync(employeeId, userId);
                if (result.IsFailure)
                    return Problem(result.Error.Description, result.Error.Code, result.Error.StatusCode);
                return Ok(result);
            }

        // ============= CEO STEP ENDPOINTS =============
        [HttpPut("ceo/update")]
        public async Task<ActionResult<EmployeeResponse>> UpdateCEOInfo([FromBody] EmployeeCEORequest request)
        {
           
                var userId = GetCurrentUserId();
                var result = await _employeeService.UpdateCEOInfoAsync(request, userId);
                if (result.IsFailure)
                    return Problem(result.Error.Description, result.Error.Code, result.Error.StatusCode);
                return Ok(result);
            }

        [HttpPost("ceo/{employeeId}/complete")]
        public async Task<ActionResult<EmployeeResponse>> CompleteCEOStep(Guid employeeId)
        {
            
                var userId = GetCurrentUserId();
                var result = await _employeeService.CompleteCEOStepAsync(employeeId, userId);
                if (result.IsFailure)
                    return Problem(result.Error.Description, result.Error.Code, result.Error.StatusCode);
                return Ok(result);
            }

        // ============= QUERY ENDPOINTS =============
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeResponse>> GetEmployee(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();

                // Check if user can access this employee
                var canAccess = await _employeeService.CanUserAccessEmployeeAsync(userId, id);
                if (!canAccess)
                    return Forbid("You don't have access to this employee");

                var result = await _employeeService.GetEmployeeByIdAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeResponse>>> GetAllEmployees()
        {
            var result = await _employeeService.GetAllEmployeesAsync();
            return Ok(result);
        }

        [HttpGet("pending-approvals")]
        public async Task<ActionResult<IEnumerable<EmployeeResponse>>> GetMyPendingApprovals()
        {
            var userId = GetCurrentUserId();
            var result = await _employeeService.GetMyPendingApprovalsAsync(userId);
            return Ok(result);
        }

        [HttpGet("by-status/{status}")]
        public async Task<ActionResult<IEnumerable<EmployeeResponse>>> GetEmployeesByStatus(string status)
        {
            var result = await _employeeService.GetEmployeesByStatusAsync(status);
            return Ok(result);
        }

        [HttpGet("employees/step/{stepName}")]
        public async Task<ActionResult<List<Employee>>> GetEmployeesByStep(string stepName)
        {
            var userId = GetCurrentUserId();
            var employees = await _employeeService.GetEmployeesByStepForUserAsync(stepName, userId);
            return Ok(employees);
        }

        [HttpGet("by-department/{departmentId}")]
        public async Task<ActionResult<IEnumerable<EmployeeResponse>>> GetEmployeesByDepartment(int departmentId)
        {
            var result = await _employeeService.GetEmployeesByDepartmentAsync(departmentId);
            return Ok(result);
        }
        [HttpGet("{employeeId}/can-edit-step/{step}")]
        public async Task<ActionResult<bool>> CanEditStep(Guid employeeId, string step)
        {
            try
            {
                var userId = GetCurrentUserId();
                var canEdit = await _employeeService.IsUserAuthorizedForStepAsync(userId, employeeId, step);
                return Ok(canEdit);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
        //[HttpGet("new-comers")]
        //public async Task<ActionResult<IEnumerable<EmployeeResponse>>> GetNewComers()
        //{
        //    // Get employees that are still in draft or in progress
        //    var newComers = await _employeeService.GetEmployeesByStatusAsync("Draft");
        //    var inProgress = await _employeeService.GetEmployeesByStatusAsync("InProgress");

        //    var allNewComers = newComers.Concat(inProgress)
        //        .OrderByDescending(e => e.CreatedAt)
        //        .ToList();

        //    return Ok(allNewComers);
        //}

        [HttpGet("statistics/workflow")]
        public async Task<ActionResult<Dictionary<string, int>>> GetWorkflowStatistics()
        {
            var result = await _employeeService.GetWorkflowStatisticsAsync();
            return Ok(result);
        }

        // ============= HELPER METHODS =============
        private string GetCurrentUserId()
        {
            // Extract user ID from JWT token or claims
            return User.GetUserId();
        }
        [HttpGet("current-user-role")]
        public async Task<ActionResult> GetCurrentUserRole()
        {
            var userId = GetCurrentUserId(); // Your method to get current user ID
            var roleInfo = await _employeeService.GetUserRoleInfoAsync(userId);
            var primaryRole = await _employeeService.GetUserPrimaryRoleAsync(userId);

            return Ok(new
            {
                primaryRole = primaryRole,
                allRoles = roleInfo.Roles,
                departmentId = roleInfo.DepartmentId,
                departmentName = roleInfo.DepartmentName
            });
        }

        [HttpGet("departments")]

        public async Task<IEnumerable<DepartmentResponse>> GetAllDepartment()
        {
            var departments = await _userService.GetAllDepartmentsAsync();
            return departments;

        }


    }
}