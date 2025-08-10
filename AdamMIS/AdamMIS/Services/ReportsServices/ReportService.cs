using AdamMIS.Authentications.Filters;
using AdamMIS.Contract.Reports;
using AdamMIS.Contract.SystemLogs;
using AdamMIS.Entities.ReportsEnitites;
using AdamMIS.Errors;
using AdamMIS.Services.LogServices;
using FastReport.Barcode;
using Newtonsoft.Json;

namespace AdamMIS.Services.ReportsServices
{
    public class ReportService : IReportService
    {

        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<ReportService> _logger;
        private readonly string _reportsPath;
        private readonly ILoggingService _loggingService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReportService(AppDbContext context, IWebHostEnvironment webHostEnvironment, ILogger<ReportService> logger
            , ILoggingService loggingservice, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _reportsPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Reports");
            _loggingService = loggingservice;
            _httpContextAccessor = httpContextAccessor;

            // Ensure Reports directory exists
            if (!Directory.Exists(_reportsPath))
            {
                Directory.CreateDirectory(_reportsPath);
            }
        }

        private string GetCurrentUsername()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
        }

        // Category Management
        public async Task<RCategoryResponse> CreateCategoryAsync(RCategoryRequest request)
        {
            var isExist = await _context.RCategories.AnyAsync(x => x.Name == request.Name);
            if (isExist)
            {
                return null;
            }

            var category = request.Adapt<RCategories>();

            await _context.RCategories.AddAsync(category);
            await _context.SaveChangesAsync();

            // Manual logging for category creation
            await _loggingService.LogAsync(new CreateLogRequest
            {
                Username = GetCurrentUsername(),
                ActionType = "Create",
                EntityName = "Report Category",
                EntityId = category.Id.ToString(),
                Description = $"Created new report category '{category.Name}'",
                OldValues = null,
                NewValues = JsonConvert.SerializeObject(new
                {
                    Name = category.Name,
                    Description = category.Description,
                    Color = category.Color,
                    CreatedAt = DateTime.UtcNow
                })
            });

            var respone = category.Adapt<RCategoryResponse>();
            return respone;
        }

        public async Task<RCategoryResponse?> GetCategoryByIdAsync(int id)
        {
            var category = await _context.RCategories.FindAsync(id);
            if (category == null)
            {
                return null;
            }

            var response = category.Adapt<RCategoryResponse>();
            return response;
        }

        public async Task<RCategoryResponse> UpdateCategoryAsync(int id, RCategoryRequest request)
        {
            var category = await _context.RCategories.FindAsync(id);
            if (category == null)
            {
                return null;
            }

            // Store old values for logging
            var oldValues = new
            {
                Name = category.Name,
                Description = category.Description,
                Color = category.Color
            };

            // Update the category properties
            category.Name = request.Name;
            category.Description = request.Description!;
            category.Color = request.Color!;

            _context.RCategories.Update(category);
            await _context.SaveChangesAsync();

            // Manual logging for category update
            await _loggingService.LogAsync(new CreateLogRequest
            {
                Username = GetCurrentUsername(),
                ActionType = "Update",
                EntityName = "Report Category",
                EntityId = category.Id.ToString(),
                Description = $"Updated report category '{category.Name}'",
                OldValues = JsonConvert.SerializeObject(oldValues),
                NewValues = JsonConvert.SerializeObject(new
                {
                    Name = category.Name,
                    Description = category.Description,
                    Color = category.Color,
                    UpdatedAt = DateTime.UtcNow
                })
            });

            var response = category.Adapt<RCategoryResponse>();
            return response;
        }

        public async Task<Result> DeleteCategoryAsync(int id)
        {
            var entity = await _context.RCategories.FindAsync(id);
            if (entity == null)
            {
                return Result.Failure(CategoryErrors.CategoryNotFound);
            }

            // Get reports count for logging
            var reports = await _context.Reports
                .Where(r => r.CategoryId == id)
                .ToListAsync();

            var reportsCount = reports.Count;

            // ✅ Delete report files from server
            foreach (var report in reports)
            {
                if (File.Exists(report.FilePath))
                {
                    File.Delete(report.FilePath);
                }
            }

            // Store category info for logging
            var categoryInfo = new
            {
                Name = entity.Name,
                Description = entity.Description,
                Color = entity.Color,
                ReportsDeleted = reportsCount
            };

            _context.RCategories.Remove(entity);
            await _context.SaveChangesAsync();

            // Manual logging for category deletion
            await _loggingService.LogAsync(new CreateLogRequest
            {
                Username = GetCurrentUsername(),
                ActionType = "Delete",
                EntityName = "Report Category",
                EntityId = id.ToString(),
                Description = $"Deleted report category '{entity.Name}' and {reportsCount} associated reports",
                OldValues = JsonConvert.SerializeObject(categoryInfo),
                NewValues = null
            });

            return Result.Success();
        }

        public async Task<IEnumerable<RCategoryResponse>> GetAllCategoriesAsync()
        {
            var categories = await _context.RCategories.AsNoTracking().ToListAsync();
            var response = categories.Adapt<IEnumerable<RCategoryResponse>>();

            return response;
        }

        // Report Management

        public async Task<ReportResponse?> GetReportByIdAsync(int id)
        {
            var report = await _context.Reports
                .Where(r => r.Id == id && r.IsActive)
                .ProjectToType<ReportResponse>() // SQL-side projection
                .FirstOrDefaultAsync();
            if (report == null)
                return null;
            return report;
        }

        public async Task<IEnumerable<ReportResponse>> GetAllReportsAsync()
        {
            var reports = await _context.Reports
            .Where(x => x.IsActive)
            .Include(r => r.Category)
            .AsNoTracking()
            .ToListAsync();
            var response = reports.Adapt<IEnumerable<ReportResponse>>();

            return response;
        }

        public async Task<IEnumerable<ReportResponse>> GetReportsByCategoryAsync(int categoryId)
        {
            var reports = await _context.Reports.Include(r => r.Category).Where(r => r.CategoryId == categoryId && r.IsActive).ToListAsync();

            var response = reports.Adapt<IEnumerable<ReportResponse>>();
            return response;

        }

        public async Task<ReportResponse> UploadReportAsync(ReportRequest request, string createdBy)
        {
            var categoryExists = await _context.RCategories.AnyAsync(c => c.Id == request.CategoryId);
            if (!categoryExists)
                return null;
            var fileExtension = Path.GetExtension(request.File.FileName);
            if (!fileExtension.Equals(".rpt", StringComparison.OrdinalIgnoreCase))
                return null;
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(_reportsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.File.CopyToAsync(stream);
            }

            var report = new Reports
            {
                FileName = request.File.FileName,
                FilePath = filePath,
                CategoryId = request.CategoryId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy,
                IsActive = true
            };
            await _context.Reports.AddAsync(report);
            await _context.SaveChangesAsync();

            var categoryName = await _context.RCategories
                .Where(c => c.Id == request.CategoryId)
                .Select(c => c.Name)
                .FirstOrDefaultAsync();

            // Manual logging for report upload
            await _loggingService.LogAsync(new CreateLogRequest
            {
                Username = GetCurrentUsername(),
                ActionType = "Create",
                EntityName = "Report",
                EntityId = report.Id.ToString(),
                Description = $"Uploaded new report '{report.FileName}' to category '{categoryName}'",
                OldValues = null,
                NewValues = JsonConvert.SerializeObject(new
                {
                    FileName = report.FileName,
                    CategoryName = categoryName,
                    FileSizeBytes = request.File.Length,
                    CreatedBy = createdBy,
                    CreatedAt = report.CreatedAt
                })
            });

            return new ReportResponse
            {
                Id = report.Id,
                FileName = report.FileName,
                FilePath = report.FilePath,
                CategoryId = report.CategoryId,
                CategoryName = categoryName ?? "",
                CreatedAt = report.CreatedAt,
                CreatedBy = report.CreatedBy,
                IsActive = report.IsActive
            };
        }

        public async Task<bool> DeleteReportAsync(int id)
        {
            var report = await _context.Reports
                .Include(r => r.Category)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (report == null)
                return false;

            // Store report info for logging
            var reportInfo = new
            {
                FileName = report.FileName,
                CategoryName = report.Category?.Name,
                CreatedBy = report.CreatedBy,
                CreatedAt = report.CreatedAt,
                FilePath = report.FilePath
            };

            if (File.Exists(report.FilePath))
            {
                File.Delete(report.FilePath);
            }
            _context.Reports.Remove(report);
            await _context.SaveChangesAsync();

            // Manual logging for report deletion
            await _loggingService.LogAsync(new CreateLogRequest
            {
                Username = GetCurrentUsername(),
                ActionType = "Delete",
                EntityName = "Report",
                EntityId = id.ToString(),
                Description = $"Deleted report '{report.FileName}' from category '{report.Category?.Name}'",
                OldValues = JsonConvert.SerializeObject(reportInfo),
                NewValues = null
            });

            return true;
        }

        // User Report Assignment

        public async Task<IEnumerable<UserReportResponse>> GetReportUsersAsync(int reportId)
        {
            var reportUsers = await _context.UserReports
                .Include(ur => ur.User)
                .Include(ur => ur.Report)
                .ThenInclude(r => r.Category)
                .Where(ur => ur.ReportId == reportId)
                .Select(ur => new UserReportResponse
                {
                    Id = ur.Id,
                    UserId = ur.UserId,
                    UserName = ur.User.UserName ?? "",
                    ReportId = ur.ReportId,
                    ReportFileName = ur.Report.FileName,
                    CategoryName = ur.Report.Category.Name,
                    AssignedAt = ur.AssignedAt,
                    AssignedBy = ur.AssignedBy,
                    IsActive = ur.Report.IsActive
                })
                .ToListAsync();
            if (reportUsers == null)
                return null;

            return reportUsers;
        }

        public async Task<IEnumerable<UserReportResponse>> GetUserReportsAsync(string userId)
        {
            var userReports = await _context.UserReports
                .Include(ur => ur.User)
                .Include(ur => ur.Report)
                .ThenInclude(r => r.Category)
                .Where(ur => ur.UserId == userId && ur.Report.IsActive)
                .Select(ur => new UserReportResponse
                {
                    Id = ur.Id,
                    UserId = ur.UserId,
                    UserName = ur.User.UserName ?? "",
                    ReportId = ur.ReportId,
                    ReportFileName = ur.Report.FileName,
                    CategoryName = ur.Report.Category.Name,
                    AssignedAt = ur.AssignedAt,
                    AssignedBy = ur.AssignedBy,
                    IsActive = ur.Report.IsActive
                })
                .ToListAsync();
            if (userReports == null)
                return null;

            return userReports;
        }

        public async Task<IEnumerable<UserReportResponse>> GetAllUserReportsAsync()
        {
            var allUserReports = await _context.UserReports
                .Include(ur => ur.User)
                .Include(ur => ur.Report)
                .ThenInclude(r => r.Category)
                .Where(ur => ur.Report.IsActive) // Only active reports
                .Select(ur => new UserReportResponse
                {
                    Id = ur.Id,
                    UserId = ur.UserId,
                    UserName = ur.User.UserName ?? "",
                    ReportId = ur.ReportId,
                    ReportFileName = ur.Report.FileName,
                    CategoryName = ur.Report.Category.Name,
                    AssignedAt = ur.AssignedAt,
                    AssignedBy = ur.AssignedBy,
                    IsActive = ur.Report.IsActive
                })
                .OrderByDescending(ur => ur.AssignedAt) // Most recent first
                .ToListAsync();

            return allUserReports;
        }

        public async Task<IEnumerable<UserReportResponse>> AssignReportsToUsersAsync(UserReportRequest request, string assignedBy)
        {
            try
            {
                var userReports = new List<UserReports>();
                var responses = new List<UserReportResponse>();

                foreach (var userId in request.UserIds)
                {
                    foreach (var reportId in request.ReportIds)
                    {
                        // Check if assignment already exists
                        var existingAssignment = await _context.UserReports
                            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.ReportId == reportId);

                        if (existingAssignment == null)
                        {
                            var userReport = new UserReports
                            {
                                UserId = userId,
                                ReportId = reportId,
                                AssignedAt = DateTime.UtcNow,
                                AssignedBy = assignedBy
                            };

                            userReports.Add(userReport);
                        }
                    }
                }

                if (userReports.Any())
                {
                    _context.UserReports.AddRange(userReports);
                    await _context.SaveChangesAsync();

                    // Get detailed information for logging
                    var assignedIds = userReports.Select(ur => ur.Id).ToList();
                    responses = await _context.UserReports
                        .Include(ur => ur.User)
                        .Include(ur => ur.Report)
                        .ThenInclude(r => r.Category)
                        .Where(ur => assignedIds.Contains(ur.Id))
                        .Select(ur => new UserReportResponse
                        {
                            Id = ur.Id,
                            UserId = ur.UserId,
                            UserName = ur.User.UserName ?? "",
                            ReportId = ur.ReportId,
                            ReportFileName = ur.Report.FileName,
                            CategoryName = ur.Report.Category.Name,
                            AssignedAt = ur.AssignedAt,
                            AssignedBy = ur.AssignedBy,
                            IsActive = ur.Report.IsActive
                        })
                        .ToListAsync();

                    // Create individual log entries for each assignment for better tracking
                    foreach (var assignment in responses)
                    {
                        await _loggingService.LogAsync(new CreateLogRequest
                        {
                            Username = GetCurrentUsername(),
                            ActionType = "Assign",
                            EntityName = "Report",
                            EntityId = $"{assignment.UserId},{assignment.ReportId}",
                            Description = $"Assigned report '{assignment.ReportFileName}' to user '{assignment.UserName}'",
                            OldValues = null,
                            NewValues = JsonConvert.SerializeObject(new
                            {
                                UserName = assignment.UserName,
                                ReportFileName = assignment.ReportFileName,
                                CategoryName = assignment.CategoryName,
                                AssignedBy = assignedBy,
                                AssignedAt = assignment.AssignedAt
                            })
                        });
                    }

                    // Also create a summary log for bulk assignments
                    if (responses.Count > 1)
                    {
                        var uniqueUsers = responses.Select(r => r.UserName).Distinct().ToList();
                        var uniqueReports = responses.Select(r => r.ReportFileName).Distinct().ToList();

                        await _loggingService.LogAsync(new CreateLogRequest
                        {
                            Username = GetCurrentUsername(),
                            ActionType = "Bulk Assign",
                            EntityName = "Report Assignment",
                            EntityId = string.Join(",", assignedIds),
                            Description = $"Bulk assigned {uniqueReports.Count} reports to {uniqueUsers.Count} users ({responses.Count} total assignments)",
                            OldValues = null,
                            NewValues = JsonConvert.SerializeObject(new
                            {
                                TotalAssignments = responses.Count,
                                UniqueUsers = uniqueUsers,
                                UniqueReports = uniqueReports,
                                AssignedBy = assignedBy,
                                AssignedAt = DateTime.UtcNow
                            })
                        });
                    }
                }

                return responses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning reports to users");
                throw;
            }
        }

        public async Task<bool> RemoveUserReportAssignmentAsync(int userReportId)
        {
            var userReport = await _context.UserReports
                .Include(ur => ur.User)
                .Include(ur => ur.Report)
                .ThenInclude(r => r.Category)
                .FirstOrDefaultAsync(ur => ur.Id == userReportId);

            if (userReport == null)
                return false;

            // Store assignment info for logging
            var assignmentInfo = new
            {
                UserName = userReport.User?.UserName,
                ReportFileName = userReport.Report?.FileName,
                CategoryName = userReport.Report?.Category?.Name,
                AssignedBy = userReport.AssignedBy,
                AssignedAt = userReport.AssignedAt
            };

            _context.UserReports.Remove(userReport);
            await _context.SaveChangesAsync();

            // Manual logging for report unassignment
            await _loggingService.LogAsync(new CreateLogRequest
            {
                Username = GetCurrentUsername(),
                ActionType = "Unassign",
                EntityName = "Report",
                EntityId = userReportId.ToString(),
                Description = $"Removed report '{userReport.Report?.FileName}' from user '{userReport.User?.UserName}'",
                OldValues = JsonConvert.SerializeObject(assignmentInfo),
                NewValues = null
            });

            return true;
        }

        public async Task<Result> ClearAllReportsAsync(CancellationToken cancellationToken)
        {
            // 1. Delete all users
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Reports]", cancellationToken);

            // 2. Reset identity seed (to start from 1)
            await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Reports', RESEED, 0)", cancellationToken);

            return Result.Success();
        }
    }
}