using AdamMIS.Contract.Reports;
using AdamMIS.Entities.ReportsEnitites;
using FastReport.Barcode;

namespace AdamMIS.Services.ReportsServices
{
    public class ReportService : IReportService
    {
      
            private readonly AppDbContext _context;
            private readonly IWebHostEnvironment _webHostEnvironment;
            private readonly ILogger<ReportService> _logger;
            private readonly string _reportsPath;

            public ReportService(AppDbContext context, IWebHostEnvironment webHostEnvironment, ILogger<ReportService> logger)
            {
                _context = context;
                _webHostEnvironment = webHostEnvironment;
                _logger = logger;
                _reportsPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Reports");

                // Ensure Reports directory exists
                if (!Directory.Exists(_reportsPath))
                {
                    Directory.CreateDirectory(_reportsPath);
                }
            }





        // Category Management
        public async Task<RCategoryResponse> CreateCategoryAsync(RCategoryRequest request)
        {
            var isExist = await _context.RCategories.AnyAsync(x=>x.Name == request.Name);
            if (isExist)
            {
                return null;
            }

            var category = request.Adapt<RCategories>();

            await _context.RCategories.AddAsync(category);
            await _context.SaveChangesAsync();

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

        public Task<RCategoryResponse> UpdateCategoryAsync(int id, RCategoryRequest request)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var entity = await _context.RCategories.FindAsync(id);
            if (entity == null)
            {
                return false;
            }

            _context.RCategories.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
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
            if(report == null)
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
            var reports = await _context.Reports.Include(r=>r.Category).Where(r=>r.CategoryId==categoryId && r.IsActive).ToListAsync();

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
            var report = await _context.Reports.FindAsync(id);
            if (report == null)
                return false;

            // Soft delete - set IsActive to false
            report.IsActive = false;
            await _context.SaveChangesAsync();

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

                    // Get response data
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
            var userReport = await _context.UserReports.FindAsync(userReportId);
            if (userReport == null)
                return false;
            _context.UserReports.Remove(userReport);
            await _context.SaveChangesAsync();
            return true;
        }




        public async Task<Result> ClearAllUsersAsync(CancellationToken cancellationToken)
        {
            // 1. Delete all users
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Reports]", cancellationToken);

            // 2. Reset identity seed (to start from 1)
            await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Reports', RESEED, 0)", cancellationToken);

            return Result.Success();
        }

    }
}
