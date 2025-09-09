using AdamMIS.Entities.DepartmentEntities;

namespace AdamMIS.Services.EmployeeServices
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _context;

        public EmployeeService(AppDbContext context)
        {
            _context = context;
        }

        // ============= HR STEP =============
        public async Task<Result<EmployeeResponse>> CreateEmployeeAsync(EmployeeHRRequest request, string createdBy)
        {
            if (!await IsUserHRHeadAsync(createdBy))
                return Result.Failure<EmployeeResponse>(EmployeeErrors.UserIsNotHr);
            var isExist = await _context.Employees.AnyAsync(x => x.EmployeeNumber == request.EmployeeNumber);
            if (isExist)
            {
                return Result.Failure<EmployeeResponse>(EmployeeErrors.DublicatedEmployee);
            }

            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                EmployeeNumber = request.EmployeeNumber,
                NameArabic = request.NameArabic,
                NameEnglish = request.NameEnglish,
                PersonalEmail = request.PersonalEmail,
                ContactPhone = request.ContactPhone,
                PayrollNumber = request.PayrollNumber,
                DepartmentId = request.DepartmentId,
                IsMedical = request.IsMedical,

                // Initialize all other fields as null
                CurrentStep = "HR",
                Status = "Draft",

                // Audit
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                CreatedBy = createdBy
            };

            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
            var response = MapToResponse(employee);
            return Result.Success(response);
        }

        public async Task<Result<EmployeeResponse>> CompleteHRStepAsync(Guid employeeId, string userId)
        {
            var employee = await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null)
                return Result.Failure<EmployeeResponse>(EmployeeErrors.EmployeeNotFound);

            if (!await IsUserHRHeadAsync(userId))
                return Result.Failure<EmployeeResponse>(EmployeeErrors.UserIsNotHr);

            // Mark HR step as completed
            employee.HRCompletedAt = DateTime.Now;
            employee.HRCompletedBy = userId;
            employee.CurrentStep = "DepartmentHead";
            employee.Status = "InProgress";
            employee.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            var response = MapToResponse(employee);
            return Result.Success(response);
        }

        // ============= DEPARTMENT HEAD STEP =============
        public async Task<Result<EmployeeResponse>> UpdateDepartmentInfoAsync(EmployeeDepartmentHeadRequest request, string userId)
        {
            var employee = await _context.Employees
                .Include(e => e.Department)
                    .ThenInclude(d => d.Heads)
                .FirstOrDefaultAsync(e => e.Id == request.EmployeeId);

            if (employee == null)
                return Result.Failure<EmployeeResponse>(EmployeeErrors.EmployeeNotFound);

            // Check if user is a head of this employee's department
            if (!employee.Department.Heads.Any(h => h.HeadId == userId))
                return Result.Failure<EmployeeResponse>(EmployeeErrors.UserIsNotDepartmentHead);

            // Update medical fields if medical employee
            if (employee.IsMedical)
            {
                employee.Qualification = request.Qualification;
                employee.Specialty = request.Specialty;
                employee.MedicalServiceCode = request.MedicalServiceCode;
                employee.DoctorStatus = request.DoctorStatus;
                employee.SeniorDoctorName = request.SeniorDoctorName;
                employee.MedicalProfileType = request.MedicalProfileType;
            }
            else
            {
                // Clear medical fields if not medical
                employee.Qualification = null;
                employee.Specialty = null;
                employee.MedicalServiceCode = null;
                employee.DoctorStatus = null;
                employee.SeniorDoctorName = null;
                employee.MedicalProfileType = null;
            }

            // Update system permissions
            employee.SystemPermissions = request.SystemPermissions;
            employee.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            var response = MapToResponse(employee);
            return Result.Success(response);
        }

        public async Task<Result<EmployeeResponse>> CompleteDepartmentStepAsync(Guid employeeId, string userId)
        {
            var employee = await _context.Employees
                .Include(e => e.Department)
                    .ThenInclude(d => d.Heads)
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null)
                return Result.Failure<EmployeeResponse>(EmployeeErrors.EmployeeNotFound);

            // Check if user is a head of this employee's department
            if (!employee.Department.Heads.Any(h => h.HeadId == userId))
                return Result.Failure<EmployeeResponse>(EmployeeErrors.UserIsNotDepartmentHead);

            employee.DepartmentCompletedAt = DateTime.Now;
            employee.DepartmentCompletedBy = userId;
            employee.CurrentStep = "IT";
            employee.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            var response = MapToResponse(employee);
            return Result.Success(response);
        }

        // ============= IT STEP =============
        public async Task<Result<EmployeeResponse>> UpdateITInfoAsync(EmployeeITRequest request, string userId)
        {
            var employee = await _context.Employees.FindAsync(request.EmployeeId);
            if (employee == null)
                return Result.Failure<EmployeeResponse>(EmployeeErrors.EmployeeNotFound);

            if (!await IsUserITHeadAsync(userId))
                return Result.Failure<EmployeeResponse>(EmployeeErrors.UserIsNotITtHead);

            employee.InternetAccess = request.InternetAccess;
            employee.ExternalEmail = request.ExternalEmail;
            employee.InternalEmail = request.InternalEmail;
            employee.FilesSharing = request.FilesSharing;
            employee.NetworkId = request.NetworkId;
            employee.EmailId = request.EmailId;
            employee.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            var response = MapToResponse(employee);
            return Result<EmployeeResponse>.Success(response);
        }

        public async Task<Result<EmployeeResponse>> CompleteITStepAsync(Guid employeeId, string userId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
                return Result.Failure<EmployeeResponse>(EmployeeErrors.EmployeeNotFound);

            if (!await IsUserITHeadAsync(userId))
                return Result.Failure<EmployeeResponse>(EmployeeErrors.UserIsNotITtHead);

            employee.ITCompletedAt = DateTime.Now;
            employee.ITCompletedBy = userId;
            employee.CurrentStep = "CEO";
            employee.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            var response = MapToResponse(employee);
            return Result.Success(response); // Fixed: removed double Result wrapping
        }

        // ============= CEO STEP =============
        public async Task<Result<EmployeeResponse>> UpdateCEOInfoAsync(EmployeeCEORequest request, string userId)
        {
            var employee = await _context.Employees.FindAsync(request.EmployeeId);
            if (employee == null)
                return Result.Failure<EmployeeResponse>(EmployeeErrors.EmployeeNotFound);

            if (!await IsUserCEOAsync(userId))
                return Result.Failure<EmployeeResponse>(EmployeeErrors.UserIsNotCEO);

            employee.CEOSignature = request.CEOSignature;
            employee.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            var response = MapToResponse(employee);
            return Result.Success(response);
        }

        public async Task<Result<EmployeeResponse>> CompleteCEOStepAsync(Guid employeeId, string userId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
                return Result.Failure<EmployeeResponse>(EmployeeErrors.EmployeeNotFound);

            if (!await IsUserCEOAsync(userId))
                return Result.Failure<EmployeeResponse>(EmployeeErrors.UserIsNotCEO);

            employee.CEOCompletedAt = DateTime.Now;
            employee.CEOCompletedBy = userId;
            employee.CurrentStep = "Completed";
            employee.Status = "Approved";
            employee.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            var response = MapToResponse(employee);
            return Result.Success(response);
        }

        // ============= QUERY METHODS =============
        public async Task<EmployeeResponse> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
                throw new KeyNotFoundException($"Employee with ID {id} not found");

            return MapToResponse(employee);
        }

        public async Task<IEnumerable<EmployeeResponse>> GetAllEmployeesAsync()
        {
            var employees = await _context.Employees
                .Include(e => e.Department)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            return employees.Select(MapToResponse);
        }

        public async Task<IEnumerable<EmployeeResponse>> GetMyPendingApprovalsAsync(string userId)
        {
            var userRole = await GetUserPrimaryRoleAsync(userId);

            IQueryable<Employee> query = _context.Employees
                .Include(e => e.Department)
                    .ThenInclude(d => d.Heads)
                .Where(e => e.Status == "InProgress" || e.Status == "Draft");

            query = userRole switch
            {
                "HR" => query.Where(e => e.CurrentStep == "HR"),
                "IT" => query.Where(e => e.CurrentStep == "IT"),
                "CEO" => query.Where(e => e.CurrentStep == "CEO"),
                _ => query.Where(e => e.CurrentStep == "DepartmentHead" &&
                                    e.Department.Heads.Any(h => h.HeadId == userId))
            };

            var employees = await query.OrderByDescending(e => e.CreatedAt).ToListAsync();
            return employees.Select(MapToResponse);
        }

        // ============= ACCESS CONTROL HELPERS =============
        private async Task<bool> IsUserHRHeadAsync(string userId)
        {
            return await _context.DepartmentHeads
                .Include(dh => dh.Department)
                .AnyAsync(dh => dh.Department.Name == "HR" && dh.HeadId == userId);
        }

        private async Task<bool> IsUserITHeadAsync(string userId)
        {
            return await _context.DepartmentHeads
                .Include(dh => dh.Department)
                .AnyAsync(dh => dh.Department.Name == "IT" && dh.HeadId == userId);
        }

        private async Task<bool> IsUserCEOAsync(string userId)
        {
            return await _context.DepartmentHeads
                .Include(dh => dh.Department)
                .AnyAsync(dh => dh.Department.Name == "CEO" && dh.HeadId == userId);
        }

        private async Task<bool> IsUserDepartmentHeadAsync(string userId)
        {
            return await _context.DepartmentHeads.AnyAsync(dh => dh.HeadId == userId);
        }

        private async Task<List<Department>> GetUserDepartmentsAsync(string userId)
        {
            return await _context.DepartmentHeads
                .Include(dh => dh.Department)
                .Where(dh => dh.HeadId == userId)
                .Select(dh => dh.Department)
                .ToListAsync();
        }

        public async Task<EmployeeRoleRequest> GetUserRoleInfoAsync(string userId)
        {
            var userDepartments = await GetUserDepartmentsAsync(userId);
            var roleInfo = new EmployeeRoleRequest();

            if (userDepartments.Any())
            {
                // For simplicity, use the first department for display
                // You might want to modify this logic based on your business needs
                var primaryDepartment = userDepartments.First();
                roleInfo.DepartmentId = primaryDepartment.Id;
                roleInfo.DepartmentName = primaryDepartment.Name;

                // All department heads have DepartmentHead role
                roleInfo.Roles.Add("DepartmentHead");

                // Add specialized roles based on departments
                foreach (var department in userDepartments)
                {
                    switch (department.Name.ToUpper())
                    {
                        case "HR":
                            roleInfo.Roles.Add("HR");
                            break;
                        case "IT":
                            roleInfo.Roles.Add("IT");
                            break;
                        case "CEO":
                            roleInfo.Roles.Add("CEO");
                            break;
                    }
                }
            }
            else
            {
                // Regular employee
                roleInfo.Roles.Add("Employee");
            }

            return roleInfo;
        }

        // Helper method to get primary role for UI display
        public async Task<string> GetUserPrimaryRoleAsync(string userId)
        {
            var roleInfo = await GetUserRoleInfoAsync(userId);

            // Return the most specific role for UI display
            if (roleInfo.Roles.Contains("HR")) return "HR";
            if (roleInfo.Roles.Contains("IT")) return "IT";
            if (roleInfo.Roles.Contains("CEO")) return "CEO";
            if (roleInfo.Roles.Contains("DepartmentHead")) return "DepartmentHead";
            return "Employee";
        }

        // Method to check if user can access a specific step
        public async Task<bool> CanUserAccessStepAsync(string userId, string stepName)
        {
            var roleInfo = await GetUserRoleInfoAsync(userId);

            return stepName.ToUpper() switch
            {
                "HR" => roleInfo.Roles.Contains("HR"),
                "DEPARTMENTHEAD" => roleInfo.Roles.Contains("DepartmentHead"),
                "IT" => roleInfo.Roles.Contains("IT"),
                "CEO" => roleInfo.Roles.Contains("CEO"),
                _ => false
            };
        }

        public async Task<List<Employee>> GetEmployeesByStepForUserAsync(string stepName, string userId)
        {
            var roleInfo = await GetUserRoleInfoAsync(userId);

            // Check if user can access this step
            if (!await CanUserAccessStepAsync(userId, stepName))
            {
                return new List<Employee>();
            }

            var query = _context.Employees
                .Include(e => e.Department)
                    .ThenInclude(d => d.Heads)
                .Where(e => e.CurrentStep == stepName);

            // ONLY filter by department for the DepartmentHead step
            if (stepName.ToUpper() == "DEPARTMENTHEAD")
            {
                // HR, IT, and CEO heads can see ALL employees in DepartmentHead step
                // Regular department heads only see their department's employees
                if (!roleInfo.Roles.Contains("HR") &&
                    !roleInfo.Roles.Contains("IT") &&
                    !roleInfo.Roles.Contains("CEO"))
                {
                    // Get all departments this user is head of
                    var userDepartmentIds = await _context.DepartmentHeads
                        .Where(dh => dh.HeadId == userId)
                        .Select(dh => dh.DepartmentId)
                        .ToListAsync();

                    query = query.Where(e => userDepartmentIds.Contains(e.DepartmentId));
                }
            }

            // For all other steps (HR, IT, CEO), return ALL employees without department filtering
            return await query.Select(e => new Employee
            {
                Id = e.Id,
                EmployeeNumber = e.EmployeeNumber,
                NameArabic = e.NameArabic,
                NameEnglish = e.NameEnglish,
                DepartmentId = e.DepartmentId,
                DepartmentName = e.Department != null ? e.Department.Name : null,
                Status = e.Status,
                CreatedAt = e.CreatedAt
            }).ToListAsync();
        }

        // ============= MAPPING =============
        private static EmployeeResponse MapToResponse(Employee employee)
        {
            return new EmployeeResponse
            {
                Id = employee.Id,
                EmployeeNumber = employee.EmployeeNumber,
                NameArabic = employee.NameArabic,
                NameEnglish = employee.NameEnglish,
                PersonalEmail = employee.PersonalEmail,
                ContactPhone = employee.ContactPhone,
                PayrollNumber = employee.PayrollNumber,
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department?.Name ?? string.Empty,
                IsMedical = employee.IsMedical,
                Qualification = employee.Qualification,
                Specialty = employee.Specialty,
                MedicalServiceCode = employee.MedicalServiceCode,
                DoctorStatus = employee.DoctorStatus,
                SeniorDoctorName = employee.SeniorDoctorName,
                MedicalProfileType = employee.MedicalProfileType,
                SystemPermissions = employee.SystemPermissions,
                InternetAccess = employee.InternetAccess,
                ExternalEmail = employee.ExternalEmail,
                InternalEmail = employee.InternalEmail,
                FilesSharing = employee.FilesSharing,
                NetworkId = employee.NetworkId,
                EmailId = employee.EmailId,
                CEOSignature = employee.CEOSignature,
                CurrentStep = employee.CurrentStep,
                Status = employee.Status,
                HRCompletedAt = employee.HRCompletedAt,
                HRCompletedBy = employee.HRCompletedBy,
                DepartmentCompletedAt = employee.DepartmentCompletedAt,
                DepartmentCompletedBy = employee.DepartmentCompletedBy,
                ITCompletedAt = employee.ITCompletedAt,
                ITCompletedBy = employee.ITCompletedBy,
                CEOCompletedAt = employee.CEOCompletedAt,
                CEOCompletedBy = employee.CEOCompletedBy,
                CreatedAt = employee.CreatedAt,
                UpdatedAt = employee.UpdatedAt,
                CreatedBy = employee.CreatedBy
            };
        }

        // ============= ADDITIONAL QUERY METHODS =============
        public async Task<IEnumerable<EmployeeResponse>> GetEmployeesByStatusAsync(string status)
        {
            var employees = await _context.Employees
                .Include(e => e.Department)
                .Where(e => e.Status == status)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            return employees.Select(MapToResponse);
        }

        public async Task<IEnumerable<EmployeeResponse>> GetEmployeesByStepAsync(string step)
        {
            var employees = await _context.Employees
                .Include(e => e.Department)
                .Where(e => e.CurrentStep == step)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            return employees.Select(MapToResponse);
        }

        public async Task<IEnumerable<EmployeeResponse>> GetEmployeesByDepartmentAsync(int departmentId)
        {
            var employees = await _context.Employees
                .Include(e => e.Department)
                .Where(e => e.DepartmentId == departmentId)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            return employees.Select(MapToResponse);
        }

        public async Task<IEnumerable<EmployeeResponse>> GetCompletedEmployeesAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.Employees
                .Include(e => e.Department)
                .Where(e => e.Status == "Approved");

            if (fromDate.HasValue)
                query = query.Where(e => e.CEOCompletedAt >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(e => e.CEOCompletedAt <= toDate.Value);

            var employees = await query
                .OrderByDescending(e => e.CEOCompletedAt)
                .ToListAsync();

            return employees.Select(MapToResponse);
        }

        // ============= ACCESS CONTROL METHODS =============
        public async Task<string> GetUserRoleForEmployeeAsync(string userId, Guid employeeId)
        {
            var employee = await _context.Employees
                .Include(e => e.Department)
                    .ThenInclude(d => d.Heads)
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null) return "None";

            // Check if user is HR head
            if (await IsUserHRHeadAsync(userId)) return "HR";

            // Check if user is a head for this employee's department
            if (employee.Department.Heads.Any(h => h.HeadId == userId)) return "DepartmentHead";

            // Check if user is IT head
            if (await IsUserITHeadAsync(userId)) return "IT";

            // Check if user is CEO
            if (await IsUserCEOAsync(userId)) return "CEO";

            return "None";
        }

        public async Task<bool> CanUserEditEmployeeStepAsync(string userId, Guid employeeId, string step)
        {
            var employee = await _context.Employees
                .Include(e => e.Department)
                    .ThenInclude(d => d.Heads)
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null) return false;

            // User can only edit if the employee is currently at the specified step
            if (employee.CurrentStep != step) return false;

            return step switch
            {
                "HR" => await IsUserHRHeadAsync(userId),
                "DepartmentHead" => employee.Department.Heads.Any(h => h.HeadId == userId),
                "IT" => await IsUserITHeadAsync(userId),
                "CEO" => await IsUserCEOAsync(userId),
                _ => false
            };
        }

        public async Task<bool> IsUserAuthorizedForStepAsync(string userId, Guid employeeId, string step)
        {
            var employee = await _context.Employees
                .Include(e => e.Department)
                    .ThenInclude(d => d.Heads)
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null) return false;

            return step switch
            {
                "HR" => await IsUserHRHeadAsync(userId),
                "DepartmentHead" => employee.Department.Heads.Any(h => h.HeadId == userId),
                "IT" => await IsUserITHeadAsync(userId),
                "CEO" => await IsUserCEOAsync(userId),
                _ => false
            };
        }

        public async Task<bool> CanUserAccessEmployeeAsync(string userId, Guid employeeId)
        {
            var userRole = await GetUserRoleForEmployeeAsync(userId, employeeId);
            return userRole != "None";
        }

        // ============= STATISTICS =============
        public async Task<Dictionary<string, int>> GetWorkflowStatisticsAsync()
        {
            var stats = new Dictionary<string, int>
            {
                ["Draft"] = await _context.Employees.CountAsync(e => e.Status == "Draft"),
                ["InProgress"] = await _context.Employees.CountAsync(e => e.Status == "InProgress"),
                ["Approved"] = await _context.Employees.CountAsync(e => e.Status == "Approved"),
                ["HR_Step"] = await _context.Employees.CountAsync(e => e.CurrentStep == "HR"),
                ["Department_Step"] = await _context.Employees.CountAsync(e => e.CurrentStep == "DepartmentHead"),
                ["IT_Step"] = await _context.Employees.CountAsync(e => e.CurrentStep == "IT"),
                ["CEO_Step"] = await _context.Employees.CountAsync(e => e.CurrentStep == "CEO"),
                ["Completed"] = await _context.Employees.CountAsync(e => e.CurrentStep == "Completed")
            };

            return stats;
        }

        public async Task<Dictionary<string, int>> GetDepartmentStatisticsAsync()
        {
            var stats = await _context.Employees
                .Include(e => e.Department)
                .GroupBy(e => e.Department.Name)
                .ToDictionaryAsync(g => g.Key, g => g.Count());

            return stats;
        }
    }
}