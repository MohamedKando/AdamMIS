
using Newtonsoft.Json;


namespace AdamMIS.Services.MetabaseServices
{


namespace AdamMIS.Services.Implementations
    {
        public class MetabaseServices : IMetabaseServices
        {
            private readonly AppDbContext _context;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly ILoggingService _loggingService;
            private readonly ILogger<MetabaseServices> _logger;

            public MetabaseServices(AppDbContext context, IHttpContextAccessor httpContextAccessor,
                ILoggingService loggingService, ILogger<MetabaseServices> logger)
            {
                _context = context;
                _httpContextAccessor = httpContextAccessor;
                _loggingService = loggingService;
                _logger = logger;
            }

            #region URL Management

            public async Task<IEnumerable<MetabaseResponse>> GetAllUrlsAsync()
            {
                var metabases = await _context.Metabases
                    .OrderByDescending(m => m.CreatedAt)
                    .AsNoTracking()
                    .ToListAsync();

                var response = metabases.Adapt<IEnumerable<MetabaseResponse>>();
                return response;
            }

            public async Task<MetabaseResponse?> GetUrlByIdAsync(int id)
            {
                var metabase = await _context.Metabases
                    .Where(m => m.Id == id)
                    .ProjectToType<MetabaseResponse>() // SQL-side projection
                    .FirstOrDefaultAsync();

                if (metabase == null)
                    return null;

                return metabase;
            }

            public async Task<MetabaseResponse> CreateUrlAsync(MetabaseRequest request, string createdBy)
            {
                var metabase = new Metabase
                {
                    Url = request.Url,
                    Title = request.Title,
                    Description = request.Description,
                    CreatedAt = DateTime.Now,
                    CreatedBy = createdBy
                };

                await _context.Metabases.AddAsync(metabase);
                await _context.SaveChangesAsync();

                // Manual logging for URL creation
                await _loggingService.LogAsync(new CreateLogRequest
                {
                    Username = GetCurrentUsername(),
                    ActionType = "Create",
                    EntityName = "Metabase URL",
                    EntityId = metabase.Id.ToString(),
                    Description = $"Created new URL '{metabase.Title}' with URL '{metabase.Url}'",
                    OldValues = null,
                    NewValues = JsonConvert.SerializeObject(new
                    {
                        Title = metabase.Title,
                        Url = metabase.Url,
                        Description = metabase.Description,
                        CreatedBy = createdBy,
                        CreatedAt = metabase.CreatedAt
                    })
                });

                return new MetabaseResponse
                {
                    Id = metabase.Id,
                    Url = metabase.Url,
                    Title = metabase.Title,
                    Description=metabase.Description,
                    CreatedAt = metabase.CreatedAt,
                    CreatedBy = metabase.CreatedBy
                };
            }

            public async Task<MetabaseResponse?> UpdateUrlAsync(int id, MetabaseRequest request)
            {
                var metabase = await _context.Metabases.FindAsync(id);
                if (metabase == null)
                    return null;

                // Store old values for logging
                var oldValues = new
                {
                    Title = metabase.Title,
                    Url = metabase.Url,
                    Description = metabase.Description
                };

                metabase.Url = request.Url;
                metabase.Title = request.Title;
                metabase.Description = request.Description;

                await _context.SaveChangesAsync();

                // Manual logging for URL update
                await _loggingService.LogAsync(new CreateLogRequest
                {
                    Username = GetCurrentUsername(),
                    ActionType = "Update",
                    EntityName = "Metabase URL",
                    EntityId = id.ToString(),
                    Description = $"Updated URL '{metabase.Title}'",
                    OldValues = JsonConvert.SerializeObject(oldValues),
                    NewValues = JsonConvert.SerializeObject(new
                    {
                        Title = metabase.Title,
                        Url = metabase.Url,
                        Description = metabase.Description
                    })
                });

                return new MetabaseResponse
                {
                    Id = metabase.Id,
                    Url = metabase.Url,
                    Title = metabase.Title,
                    CreatedAt = metabase.CreatedAt,
                    Description = metabase.Description,
                    CreatedBy = metabase.CreatedBy
                };
            }

            public async Task<bool> DeleteUrlAsync(int id)
            {
                var metabase = await _context.Metabases
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (metabase == null)
                    return false;

                // Store URL info for logging
                var urlInfo = new
                {
                    Title = metabase.Title,
                    Url = metabase.Url,
                    Description = metabase.Description,
                    CreatedBy = metabase.CreatedBy,
                    CreatedAt = metabase.CreatedAt
                };

                // Remove all user assignments first
                var assignments = await _context.UsersMetabases
                    .Where(um => um.MetabaseId == id)
                    .ToListAsync();

                if (assignments.Any())
                {
                    _context.UsersMetabases.RemoveRange(assignments);
                }

                _context.Metabases.Remove(metabase);
                await _context.SaveChangesAsync();

                // Manual logging for URL deletion
                await _loggingService.LogAsync(new CreateLogRequest
                {
                    Username = GetCurrentUsername(),
                    ActionType = "Delete",
                    EntityName = "Metabase URL",
                    EntityId = id.ToString(),
                    Description = $"Deleted URL '{metabase.Title}' with URL '{metabase.Url}'",
                    OldValues = JsonConvert.SerializeObject(urlInfo),
                    NewValues = null
                });

                return true;
            }

            #endregion

            #region User-URL Assignment Management

            public async Task<IEnumerable<UserMetabaseResponse>> GetUrlAssignmentsAsync(int metabaseId)
            {
                var urlUsers = await _context.UsersMetabases
                    .Include(um => um.User)
                    .Include(um => um.MetaBase)
                    .Where(um => um.MetabaseId == metabaseId)
                    .Select(um => new UserMetabaseResponse
                    {
                        Id = um.Id,
                        UserId = um.UserId,
                        UserName = um.User.UserName ?? "",
                        MetabaseId = um.MetabaseId,
                        MetabaseTitle = um.MetaBase.Title,
                        MetabaseUrl = um.MetaBase.Url,
                        AssignedAt = um.AssignedAt,
                        AssignedBy = um.AssignedBy,
                        Description = um.Description
                    })
                    .ToListAsync();

                if (urlUsers == null)
                    return new List<UserMetabaseResponse>();

                return urlUsers;
            }

            public async Task<UserMetabaseResponse?> GetUserAssignedUrlsAsync(string userId)
            {
                var userUrl = await _context.UsersMetabases
                    .Include(um => um.User)
                    .Include(um => um.MetaBase)
                    .Where(um => um.UserId == userId)
                    .Select(um => new UserMetabaseResponse
                    {
                        Id = um.Id,
                        UserId = um.UserId,
                        UserName = um.User.UserName ?? "",
                        MetabaseId = um.MetabaseId,
                        MetabaseTitle = um.MetaBase.Title,
                        MetabaseUrl = um.MetaBase.Url,
                        AssignedAt = um.AssignedAt,
                        AssignedBy = um.AssignedBy,
                        Description=um.Description
                    })
                    .FirstOrDefaultAsync();

                if (userUrl == null)
                    return null;

                return userUrl;
            }

            public async Task<IEnumerable<UserMetabaseResponse>> GetAllUsersUrlsAsync()
            {
                var allUserUrls = await _context.UsersMetabases
                    .Include(um => um.User)
                    .Include(um => um.MetaBase)
                    .Select(um => new UserMetabaseResponse
                    {
                        Id = um.Id,
                        UserId = um.UserId,
                        UserName = um.User.UserName ?? "",
                        MetabaseId = um.MetabaseId,
                        MetabaseTitle = um.MetaBase.Title,
                        MetabaseUrl = um.MetaBase.Url,
                        AssignedAt = um.AssignedAt,
                        AssignedBy = um.AssignedBy,
                        Description= um.Description,
                    })
                    .OrderByDescending(um => um.AssignedAt) // Most recent first
                    .ToListAsync();

                return allUserUrls;
            }

            public async Task<IEnumerable<UserMetabaseResponse>> GetAllAssignmentsAsync()
            {
                var allAssignments = await _context.UsersMetabases
                    .Include(um => um.User)
                    .Include(um => um.MetaBase)
                    .Select(um => new UserMetabaseResponse
                    {
                        Id = um.Id,
                        UserId = um.UserId,
                        UserName = um.User.UserName ?? "",
                        MetabaseId = um.MetabaseId,
                        MetabaseTitle = um.MetaBase.Title,
                        MetabaseUrl = um.MetaBase.Url,
                        AssignedAt = um.AssignedAt,
                        AssignedBy = um.AssignedBy,
                        Description=um.Description
                    })
                    .OrderByDescending(um => um.AssignedAt)
                    .ToListAsync();

                return allAssignments;
            }


            public async Task<AssignmentResult> AssignUrlsToUsersAsync(UserMetabaseRequest request, string assignedBy)
            {
                try
                {
                    var userMetabases = new List<UsersMetabases>();
                    var responses = new List<UserMetabaseResponse>();
                    var duplicateAssignments = new List<DuplicateAssignment>();

                    foreach (var userId in request.UserIds)
                    {
                        // Get user name for better error messages
                        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                        var userName = user?.UserName ?? userId;

                        foreach (var metabaseId in request.MetabaseIds)
                        {
                            // Get URL title for better error messages
                            var metabase = await _context.Metabases.FirstOrDefaultAsync(m => m.Id == metabaseId);
                            var metabaseTitle = metabase?.Title ?? "Unknown URL";

                            // Check if assignment already exists
                            var existingAssignment = await _context.UsersMetabases
                                .FirstOrDefaultAsync(um => um.UserId == userId && um.MetabaseId == metabaseId);

                            if (existingAssignment == null)
                            {
                                var userMetabase = new UsersMetabases
                                {
                                    UserId = userId,
                                    MetabaseId = metabaseId,
                                    AssignedAt = DateTime.Now,
                                    AssignedBy = assignedBy
                                };

                                userMetabases.Add(userMetabase);
                            }
                            else
                            {
                                // Track duplicate assignments
                                duplicateAssignments.Add(new DuplicateAssignment
                                {
                                    UserId = userId,
                                    UserName = userName,
                                    MetabaseId = metabaseId,
                                    MetabaseTitle = metabaseTitle,
                                    ExistingAssignedAt = existingAssignment.AssignedAt,
                                    ExistingAssignedBy = existingAssignment.AssignedBy
                                });
                            }
                        }
                    }

                    if (userMetabases.Any())
                    {
                        _context.UsersMetabases.AddRange(userMetabases);
                        await _context.SaveChangesAsync();

                        // Get detailed information for logging
                        var assignedIds = userMetabases.Select(um => um.Id).ToList();
                        responses = await _context.UsersMetabases
                            .Include(um => um.User)
                            .Include(um => um.MetaBase)
                            .Where(um => assignedIds.Contains(um.Id))
                            .Select(um => new UserMetabaseResponse
                            {
                                Id = um.Id,
                                UserId = um.UserId,
                                UserName = um.User.UserName ?? "",
                                MetabaseId = um.MetabaseId,
                                MetabaseTitle = um.MetaBase.Title,
                                MetabaseUrl = um.MetaBase.Url,
                                AssignedAt = um.AssignedAt,
                                AssignedBy = um.AssignedBy,
                                Description = um.Description
                            })
                            .ToListAsync();

                        // Create individual log entries for each assignment for better tracking
                        foreach (var assignment in responses)
                        {
                            await _loggingService.LogAsync(new CreateLogRequest
                            {
                                Username = GetCurrentUsername(),
                                ActionType = "Assign",
                                EntityName = "Metabase URL",
                                EntityId = $"{assignment.UserId},{assignment.MetabaseId}",
                                Description = $"Assigned URL '{assignment.MetabaseTitle}' to user '{assignment.UserName}'",
                                OldValues = null,
                                NewValues = JsonConvert.SerializeObject(new
                                {
                                    UserName = assignment.UserName,
                                    UrlTitle = assignment.MetabaseTitle,
                                    UrlAddress = assignment.MetabaseUrl,
                                    AssignedBy = assignedBy,
                                    AssignedAt = assignment.AssignedAt
                                })
                            });
                        }

                        // Also create a summary log for bulk assignments
                        if (responses.Count > 1)
                        {
                            var uniqueUsers = responses.Select(r => r.UserName).Distinct().ToList();
                            var uniqueUrls = responses.Select(r => r.MetabaseTitle).Distinct().ToList();

                            await _loggingService.LogAsync(new CreateLogRequest
                            {
                                Username = GetCurrentUsername(),
                                ActionType = "Bulk Assign",
                                EntityName = "Metabase URL Assignment",
                                EntityId = string.Join(",", assignedIds),
                                Description = $"Bulk assigned {uniqueUrls.Count} URLs to {uniqueUsers.Count} users ({responses.Count} total assignments)",
                                OldValues = null,
                                NewValues = JsonConvert.SerializeObject(new
                                {
                                    TotalAssignments = responses.Count,
                                    UniqueUsers = uniqueUsers,
                                    UniqueUrls = uniqueUrls,
                                    AssignedBy = assignedBy,
                                    AssignedAt = DateTime.Now
                                })
                            });
                        }
                    }

                    return new AssignmentResult
                    {
                        NewAssignments = responses,
                        DuplicateAssignments = duplicateAssignments,
                        TotalRequested = request.UserIds.Count * request.MetabaseIds.Count,
                        SuccessfulAssignments = responses.Count,
                        DuplicateCount = duplicateAssignments.Count
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error assigning URLs to users");
                    throw;
                }
            }

            public async Task<bool> UnassignUrlsFromUsersAsync(int userMetabaseId)
            {
                var userMetabase = await _context.UsersMetabases
                    .Include(um => um.User)
                    .Include(um => um.MetaBase)
                    .FirstOrDefaultAsync(um => um.Id == userMetabaseId);

                if (userMetabase == null)
                    return false;

                // Store assignment info for logging
                var assignmentInfo = new
                {
                    UserName = userMetabase.User?.UserName,
                    UrlTitle = userMetabase.MetaBase?.Title,
                    UrlAddress = userMetabase.MetaBase?.Url,
                    AssignedBy = userMetabase.AssignedBy,
                    AssignedAt = userMetabase.AssignedAt
                };

                _context.UsersMetabases.Remove(userMetabase);
                await _context.SaveChangesAsync();

                // Manual logging for URL unassignment
                await _loggingService.LogAsync(new CreateLogRequest
                {
                    Username = GetCurrentUsername(),
                    ActionType = "Unassign",
                    EntityName = "Metabase URL",
                    EntityId = userMetabaseId.ToString(),
                    Description = $"Removed URL '{userMetabase.MetaBase?.Title}' from user '{userMetabase.User?.UserName}'",
                    OldValues = JsonConvert.SerializeObject(assignmentInfo),
                    NewValues = null
                });

                return true;
            }

            #endregion

            #region Private Helper Methods

            private string GetCurrentUsername()
            {
                return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value ?? "System";
            }

            #endregion
        }
    }
}

