

using AdamMIS.Authentications.Filters;
using AdamMIS.Contract.Reports;
using AdamMIS.Entities.ReportsEnitites;
using AdamMIS.Services.ReportsServices;
using FastReport;
using FastReport.Export.PdfSimple;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace AdamMIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
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
       // [HasPermission(Permissions.ReadCategories)]
        public async Task<ActionResult<IEnumerable<RCategoryResponse>>> GetAllCategories()
        {
           
                var categories = await _reportService.GetAllCategoriesAsync();
                return Ok(categories);
            
        }

        [HttpGet("categories/{id}")]
        [HasPermission(Permissions.ReadCategories)]
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
        [HasPermission(Permissions.AddCategories)]
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
        [HttpDelete("categories/{id}")]
        [HasPermission(Permissions.DeleteCategories)]
        public async Task<ActionResult> DeleteCategory(int id)
        {
           
                var result = await _reportService.DeleteCategoryAsync(id);
                if (result.IsFailure)
                    return Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.Code, detail: result.Error.Description);

                return Ok(true);
            

        }

        [HttpPut("categories/{id}")]
        public async Task<ActionResult<RCategoryResponse>> UpdateCategory(int id, [FromBody] RCategoryRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var category = await _reportService.UpdateCategoryAsync(id, request);
                return Ok(category);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category {CategoryId}", id);
                return StatusCode(500, "Internal server error");
            }
        }


        //#region Report Management

        [HttpGet("reports")]
       // [HasPermission(Permissions.ReadReports)]
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
        [HasPermission(Permissions.ReadReports)]
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
        [HasPermission(Permissions.ReadReports)]
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
        [HasPermission(Permissions.UpdateReports)]
        public async Task<ActionResult<ReportResponse>> UploadReport([FromForm] ReportRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var createdBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
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
        [HasPermission(Permissions.DeleteReports)]
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
        [HasPermission(Permissions.AddReports)]
        public async Task<ActionResult<IEnumerable<UserReportResponse>>> AssignReportsToUsers([FromBody] UserReportRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var assignedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
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
        [HasPermission(Permissions.ReadReports)]
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


        [HttpGet("user-reports/users")]
        [HasPermission(Permissions.ReadReports)]
        public async Task<ActionResult<IEnumerable<UserReportResponse>>> GetUserAllReports()
        {
            try
            {
                var userReports = await _reportService.GetAllUserReportsAsync();
                return Ok(userReports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user reports for user {UserId}");
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











        [HttpPost("reports/{reportId}/generate")]
        public async Task<IActionResult> ViewReport(int reportId)
        {
            try
            {
                var report = await _reportService.GetReportByIdAsync(reportId);
                if (report == null)
                    return NotFound(new { message = "Report not found." });

                if (string.IsNullOrEmpty(report.FilePath) || !System.IO.File.Exists(report.FilePath))
                    return NotFound(new { message = "Report file not found." });

                // Instead of EXE, return a redirect (or a link) to the Web Viewer
                var viewerUrl = $"http://192.168.1.203:8090/Viewer.aspx?={Uri.EscapeDataString(report.FilePath)}";

                return Ok(new { message = "Report ready", reportId, viewerUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating report for ID {ReportId}", reportId);
                return StatusCode(500, new { message = "An error occurred while generating the report." });
            }
        }






        [HttpPost("reports/{reportId}/edit")]
        public async Task<IActionResult> OpenReportFile(int reportId)
        {
            try
            {
                // Step 1: Get the report details from database
                var report = await _reportService.GetReportByIdAsync(reportId);
                if (report == null)
                    return NotFound(new { message = "Report not found." });

                // Step 2: Use the FilePath stored in the database
                var originalFilePath = report.FilePath;

                // Step 3: Validate the original file exists
                if (string.IsNullOrEmpty(originalFilePath) || !System.IO.File.Exists(originalFilePath))
                {
                    _logger.LogError($"Report file not found at path: {originalFilePath}");
                    return NotFound(new { message = "Report file not found on server." });
                }

                // Step 4: Validate file extension
                var fileExtension = Path.GetExtension(originalFilePath)?.ToLowerInvariant();
                if (fileExtension != ".rpt")
                {
                    _logger.LogError($"Invalid file type: {fileExtension}");
                    return BadRequest(new { message = "Invalid report file type." });
                }

                // Step 5: Open the file
                Process.Start(new ProcessStartInfo
                {
                    FileName = "\\\\192.168.1.203\\c$\\Program Files\\Microsoft Visual Studio\\2022\\Community\\Common7\\IDE\\devenv.exe",
                    Arguments = $"\"{originalFilePath}\"",
                    UseShellExecute = true
                });

                _logger.LogInformation($"Opened report file: {originalFilePath}");

                return Ok(new
                {
                    message = "Report opened successfully.",
                    reportId = reportId,
                    fileName = report.FileName,
                    filePath = originalFilePath,
                    openedAt = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error opening report for ID {ReportId}", reportId);
                return StatusCode(500, new { message = "An error occurred while opening the report." });
            }
        }



        [HttpDelete("ClearAll")]
        [HasPermission(Permissions.DeleteReports)]
        public async Task<IActionResult> ClearUsers(CancellationToken cancellationToken)
        {

            var result = await _reportService.ClearAllReportsAsync(cancellationToken);

            if (result.IsFailure)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.Code, detail: result.Error.Description);

            }
            else
                return NoContent();

        }

    }
}
