

using AdamMIS.Contract.Reports;
using AdamMIS.Services.ReportsServices;
using FastReport;
using FastReport.Export.PdfSimple;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AdamMIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(IReportService reportService, ILogger<ReportsController> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        //#region Category Management

        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<RCategoryResponse>>> GetAllCategories()
        {
            try
            {
                var categories = await _reportService.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all categories");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("categories/{id}")]
        public async Task<ActionResult<RCategoryResponse>> GetCategoryById(int id)
        {
            try
            {
                var category = await _reportService.GetCategoryByIdAsync(id);
                if (category == null)
                    return NotFound("Category not found");

                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category {CategoryId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("categories")]
        public async Task<ActionResult<RCategoryResponse>> CreateCategory([FromBody] RCategoryRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var category = await _reportService.CreateCategoryAsync(request);
                return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                return StatusCode(500, "Internal server error");
            }
        }

        //[HttpPut("categories/{id}")]
        //public async Task<ActionResult<RCategoryResponse>> UpdateCategory(int id, [FromBody] RCategoryRequest request)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //            return BadRequest(ModelState);

        //        var category = await _reportService.UpdateCategoryAsync(id, request);
        //        return Ok(category);
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        return NotFound(ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error updating category {CategoryId}", id);
        //        return StatusCode(500, "Internal server error");
        //    }
        //}


        //#region Report Management

        [HttpGet("reports")]
        public async Task<ActionResult<IEnumerable<ReportResponse>>> GetAllReports()
        {
            try
            {
                var reports = await _reportService.GetAllReportsAsync();
                return Ok(reports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all reports");
                return StatusCode(500, "Internal server error");
            }
        }



        [HttpGet("reports/{id}")]
        public async Task<ActionResult<ReportResponse>> GetReportById(int id)
        {
            try
            {
                var report = await _reportService.GetReportByIdAsync(id);
                if (report == null)
                    return NotFound("Report not found");

                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting report {ReportId}", id);
                return StatusCode(500, "Internal server error");
            }
        }



        [HttpGet("reports/category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<ReportResponse>>> GetReportsByCategory(int categoryId)
        {
            try
            {
                var reports = await _reportService.GetReportsByCategoryAsync(categoryId);
                return Ok(reports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reports by category {CategoryId}", categoryId);
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPost("reports/upload")]
        public async Task<ActionResult<ReportResponse>> UploadReport([FromForm] ReportRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var createdBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
                var report = await _reportService.UploadReportAsync(request, createdBy);

                return CreatedAtAction(nameof(GetReportById), new { id = report.Id }, report);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading report");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpDelete("reports/{id}")]
        public async Task<ActionResult> DeleteReport(int id)
        {
            try
            {
                var result = await _reportService.DeleteReportAsync(id);
                if (!result)
                    return NotFound("Report not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting report {ReportId}", id);
                return StatusCode(500, "Internal server error");
            }
        }





        //#region User Report Assignment

        [HttpPost("user-reports/assign")]
        public async Task<ActionResult<IEnumerable<UserReportResponse>>> AssignReportsToUsers([FromBody] UserReportRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var assignedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
                var assignments = await _reportService.AssignReportsToUsersAsync(request, assignedBy);

                return Ok(assignments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning reports to users");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpDelete("user-reports/{userReportId}")]
        public async Task<ActionResult> RemoveUserReportAssignment(int userReportId)
        {
            try
            {
                var result = await _reportService.RemoveUserReportAssignmentAsync(userReportId);
                if (!result)
                    return NotFound("User report assignment not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing user report assignment {UserReportId}", userReportId);
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet("user-reports/user/{userId}")]
        public async Task<ActionResult<IEnumerable<UserReportResponse>>> GetUserReports(string userId)
        {
            try
            {
                var userReports = await _reportService.GetUserReportsAsync(userId);
                return Ok(userReports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user reports for user {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet("user-reports/report/{reportId}")]
        public async Task<ActionResult<IEnumerable<UserReportResponse>>> GetReportUsers(int reportId)
        {
            try
            {
                var reportUsers = await _reportService.GetReportUsersAsync(reportId);
                return Ok(reportUsers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting report users for report {ReportId}", reportId);
                return StatusCode(500, "Internal server error");
            }
        }














        [HttpDelete("ClearAll")]
        public async Task<IActionResult> ClearUsers(CancellationToken cancellationToken)
        {

            var result = await _reportService.ClearAllUsersAsync(cancellationToken);

            if (result.IsFailure)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.Code, detail: result.Error.Description);

            }
            else
                return NoContent();

        }

    }
}
