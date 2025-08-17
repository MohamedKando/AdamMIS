using AdamMIS.Contract.Metabase;
using AdamMIS.Services.MetabaseServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AdamMIS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class MetabaseController : ControllerBase
    {
        private readonly IMetabaseServices _metabaseService;
        private readonly ILogger<MetabaseController> _logger;

        public MetabaseController(IMetabaseServices metabaseService, ILogger<MetabaseController> logger)
        {
            _metabaseService = metabaseService;
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<MetabaseResponse>>> GetAllUrls()
        {
            try
            {
                var urls = await _metabaseService.GetAllUrlsAsync();
                return Ok(urls);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all URLs");
                return StatusCode(500, "An error occurred while retrieving URLs.");
            }
        }

        /// <summary>
        /// Get URL by ID
        /// </summary>
        //[HttpGet("{id}")]
        //public async Task<ActionResult<MetabaseResponse>> GetUrlById(int id)
        //{
        //    try
        //    {
        //        var url = await _metabaseService.GetUrlByIdAsync(id);
        //        if (url == null)
        //            return NotFound($"URL with ID {id} not found.");

        //        return Ok(url);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error retrieving URL with ID {UrlId}", id);
        //        return StatusCode(500, "An error occurred while retrieving the URL.");
        //    }
        //}


        [HttpPost("")]
        public async Task<ActionResult<MetabaseResponse>> CreateUrl([FromBody] MetabaseRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var createdBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
                var createdUrl = await _metabaseService.CreateUrlAsync(request, createdBy);

                return CreatedAtAction(nameof(GetAllUrls), new { id = createdUrl.Id }, createdUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating URL");
                return StatusCode(500, "An error occurred while creating the URL.");
            }
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<MetabaseResponse>> UpdateUrl(int id, [FromBody] MetabaseRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var updatedUrl = await _metabaseService.UpdateUrlAsync(id, request);
                if (updatedUrl == null)
                    return NotFound($"URL with ID {id} not found.");

                return Ok(updatedUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating URL with ID {UrlId}", id);
                return StatusCode(500, "An error occurred while updating the URL.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUrl(int id)
        {
            try
            {
                var result = await _metabaseService.DeleteUrlAsync(id);
                if (!result)
                    return NotFound($"URL with ID {id} not found.");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting URL with ID {UrlId}", id);
                return StatusCode(500, "An error occurred while deleting the URL.");
            }
        }




        [HttpGet("{metabaseId}/users")]
        public async Task<ActionResult<IEnumerable<UserMetabaseResponse>>> GetUrlAssignments(int metabaseId)
        {
            try
            {
                var assignments = await _metabaseService.GetUrlAssignmentsAsync(metabaseId);
                return Ok(assignments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving assignments for URL with ID {MetabaseId}", metabaseId);
                return StatusCode(500, "An error occurred while retrieving URL assignments.");
            }
        }


        /// Get URLs assigned to a specific user

        [HttpGet("users/{userId}")]
        public async Task<ActionResult<UserMetabaseResponse>> GetUserAssignedUrls(string userId)
        {
            try
            {
                var userUrls = await _metabaseService.GetUserAssignedUrlsAsync(userId);
                if (userUrls == null)
                    return NotFound($"No URLs found for user with ID {userId}.");

                return Ok(userUrls);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving URLs for user with ID {UserId}", userId);
                return StatusCode(500, "An error occurred while retrieving user URLs.");
            }
        }


        /// Get all users and their assigned URLs

        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UserMetabaseResponse>>> GetAllUsersUrls()
        {
            try
            {
                var allUsersUrls = await _metabaseService.GetAllUsersUrlsAsync();
                return Ok(allUsersUrls);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users' URLs");
                return StatusCode(500, "An error occurred while retrieving all users' URLs.");
            }
        }
        /// Get all assignments

        [HttpGet("assignments")]
        public async Task<ActionResult<IEnumerable<UserMetabaseResponse>>> GetAllAssignments()
        {
            try
            {
                var assignments = await _metabaseService.GetAllAssignmentsAsync();
                return Ok(assignments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all assignments");
                return StatusCode(500, "An error occurred while retrieving all assignments.");
            }
        }


        /// Assign URLs to users

        [HttpPost("assign")]
        public async Task<ActionResult<IEnumerable<UserMetabaseResponse>>> AssignUrlsToUsers([FromBody] UserMetabaseRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var assignedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
                var assignments = await _metabaseService.AssignUrlsToUsersAsync(request, assignedBy);

                return Ok(assignments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning URLs to users");
                return StatusCode(500, "An error occurred while assigning URLs to users.");
            }
        }


        /// Remove URL assignment from user

        [HttpDelete("assignments/{userMetabaseId}")]
        public async Task<ActionResult> UnassignUrlFromUser(int userMetabaseId)
        {
            try
            {
                var result = await _metabaseService.UnassignUrlsFromUsersAsync(userMetabaseId);
                if (!result)
                    return NotFound($"Assignment with ID {userMetabaseId} not found.");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing assignment with ID {AssignmentId}", userMetabaseId);
                return StatusCode(500, "An error occurred while removing the assignment.");
            }
        }



    }
}