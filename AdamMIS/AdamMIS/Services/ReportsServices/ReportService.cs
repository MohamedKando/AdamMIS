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
            // Modified: Use network path instead of local path
            _reportsPath = @"\\192.168.1.203\e$\crystal_reports";
            _loggingService = loggingservice;
            _httpContextAccessor = httpContextAccessor;

            // Ensure Reports directory exists on network path
            try
            {
                if (!Directory.Exists(_reportsPath))
                {
                    Directory.CreateDirectory(_reportsPath);
                    _logger.LogInformation($"Created reports directory at network path: {_reportsPath}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to access or create network reports directory: {_reportsPath}");
                throw new InvalidOperationException($"Cannot access network reports directory: {_reportsPath}", ex);
            }
        }

        private string GetCurrentUsername()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";
        }

        /// <summary>
        /// Validates network path accessibility
        /// </summary>
        private async Task<bool> ValidateNetworkPathAsync()
        {
            try
            {
                return await Task.Run(() => Directory.Exists(_reportsPath));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Network path validation failed: {_reportsPath}");
                return false;
            }
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

            // Modified: Delete report files from network path
            foreach (var report in reports)
            {
                try
                {
                    if (File.Exists(report.FilePath))
                    {
                        await Task.Run(() => File.Delete(report.FilePath));
                        _logger.LogInformation($"Deleted report file from network path: {report.FilePath}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to delete report file from network path: {report.FilePath}");
                    // Continue with database cleanup even if file deletion fails
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
            // Validate network path accessibility
            if (!await ValidateNetworkPathAsync())
            {
                _logger.LogError($"Network path is not accessible: {_reportsPath}");
                return null;
            }

            var categoryExists = await _context.RCategories.AnyAsync(c => c.Id == request.CategoryId);
            if (!categoryExists)
                return null;

            var fileExtension = Path.GetExtension(request.File.FileName);
            if (!fileExtension.Equals(".rpt", StringComparison.OrdinalIgnoreCase))
                return null;

            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(_reportsPath, fileName);

            try
            {
                // Modified: Save to network path with async operation and error handling
                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                {
                    await request.File.CopyToAsync(stream);
                }

                _logger.LogInformation($"Successfully saved report file to network path: {filePath}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to save report file to network path: {filePath}");
                return null;
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
                Description = $"Uploaded new report '{report.FileName}' to category '{categoryName}' on network path",
                OldValues = null,
                NewValues = JsonConvert.SerializeObject(new
                {
                    FileName = report.FileName,
                    CategoryName = categoryName,
                    FileSizeBytes = request.File.Length,
                    CreatedBy = createdBy,
                    CreatedAt = report.CreatedAt,
                    NetworkPath = filePath
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

            // Modified: Delete file from network path with error handling
            try
            {
                if (File.Exists(report.FilePath))
                {
                    await Task.Run(() => File.Delete(report.FilePath));
                    _logger.LogInformation($"Successfully deleted report file from network path: {report.FilePath}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to delete report file from network path: {report.FilePath}");
                // Continue with database deletion even if file deletion fails
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
                Description = $"Deleted report '{report.FileName}' from category '{report.Category?.Name}' and network path",
                OldValues = JsonConvert.SerializeObject(reportInfo),
                NewValues = null
            });

            return true;
        }

        /// <summary>
        /// New method: Load report file from network path
        /// </summary>
        public async Task<byte[]?> LoadReportFileAsync(int reportId)
        {
            var report = await _context.Reports
                .Where(r => r.Id == reportId && r.IsActive)
                .FirstOrDefaultAsync();

            if (report == null)
            {
                _logger.LogWarning($"Report with ID {reportId} not found or inactive");
                return null;
            }

            try
            {
                if (File.Exists(report.FilePath))
                {
                    var fileBytes = await File.ReadAllBytesAsync(report.FilePath);
                    _logger.LogInformation($"Successfully loaded report file from network path: {report.FilePath}");

                    // Log file access
                    await _loggingService.LogAsync(new CreateLogRequest
                    {
                        Username = GetCurrentUsername(),
                        ActionType = "Access",
                        EntityName = "Report",
                        EntityId = reportId.ToString(),
                        Description = $"Loaded report file '{report.FileName}' from network path",
                        OldValues = null,
                        NewValues = JsonConvert.SerializeObject(new
                        {
                            FileName = report.FileName,
                            FilePath = report.FilePath,
                            AccessedAt = DateTime.UtcNow,
                            FileSizeBytes = fileBytes.Length
                        })
                    });

                    return fileBytes;
                }
                else
                {
                    _logger.LogError($"Report file not found at network path: {report.FilePath}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to load report file from network path: {report.FilePath}");
                return null;
            }
        }

        /// <summary>
        /// New method: Get report file stream from network path
        /// </summary>
        public async Task<FileStream?> GetReportFileStreamAsync(int reportId)
        {
            var report = await _context.Reports
                .Where(r => r.Id == reportId && r.IsActive)
                .FirstOrDefaultAsync();

            if (report == null)
            {
                _logger.LogWarning($"Report with ID {reportId} not found or inactive");
                return null;
            }

            try
            {
                if (File.Exists(report.FilePath))
                {
                    var fileStream = new FileStream(report.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    _logger.LogInformation($"Successfully opened report file stream from network path: {report.FilePath}");

                    // Log file access
                    await _loggingService.LogAsync(new CreateLogRequest
                    {
                        Username = GetCurrentUsername(),
                        ActionType = "Stream Access",
                        EntityName = "Report",
                        EntityId = reportId.ToString(),
                        Description = $"Opened file stream for report '{report.FileName}' from network path",
                        OldValues = null,
                        NewValues = JsonConvert.SerializeObject(new
                        {
                            FileName = report.FileName,
                            FilePath = report.FilePath,
                            AccessedAt = DateTime.UtcNow
                        })
                    });

                    return fileStream;
                }
                else
                {
                    _logger.LogError($"Report file not found at network path: {report.FilePath}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to open report file stream from network path: {report.FilePath}");
                return null;
            }
        }

        /// <summary>
        /// New method: Check if report file exists on network path
        /// </summary>
        public async Task<bool> ReportFileExistsAsync(int reportId)
        {
            var report = await _context.Reports
                .Where(r => r.Id == reportId && r.IsActive)
                .Select(r => r.FilePath)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(report))
                return false;

            try
            {
                return await Task.Run(() => File.Exists(report));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking if report file exists: {report}");
                return false;
            }
        }

        // User Report Assignment (unchanged methods)
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
            // Modified: Also clean up files from network path before clearing database
            try
            {
                var reports = await _context.Reports.ToListAsync(cancellationToken);

                // Delete all physical files from network path
                foreach (var report in reports)
                {
                    try
                    {
                        if (File.Exists(report.FilePath))
                        {
                            await Task.Run(() => File.Delete(report.FilePath), cancellationToken);
                            _logger.LogInformation($"Deleted report file from network path: {report.FilePath}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to delete report file: {report.FilePath}");
                        // Continue with other files even if one fails
                    }
                }

                // Log the cleanup operation
                await _loggingService.LogAsync(new CreateLogRequest
                {
                    Username = GetCurrentUsername(),
                    ActionType = "Bulk Delete",
                    EntityName = "Report",
                    EntityId = "ALL",
                    Description = $"Cleared all {reports.Count} reports from database and network path",
                    OldValues = JsonConvert.SerializeObject(new
                    {
                        TotalReportsDeleted = reports.Count,
                        NetworkPath = _reportsPath,
                        DeletedAt = DateTime.UtcNow
                    }),
                    NewValues = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while cleaning up report files from network path");
            }

            // 1. Delete all reports
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Reports]", cancellationToken);

            // 2. Reset identity seed (to start from 1)
            await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Reports', RESEED, 0)", cancellationToken);

            return Result.Success();
        }
    }
}