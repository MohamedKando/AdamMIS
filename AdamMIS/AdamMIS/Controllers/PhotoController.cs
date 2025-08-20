namespace AdamMIS.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public class PhotoController : ControllerBase
    {
        private readonly string _photoDirectory = @"\\192.168.1.203\e$\App-data\user-photos";
        private readonly ILogger<PhotoController> _logger;

        public PhotoController(ILogger<PhotoController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{fileName}")]
        public async Task<IActionResult> GetPhoto(string fileName, [FromQuery] string? t = null)
        {
            try
            {
                // Sanitize filename to prevent directory traversal attacks
                fileName = Path.GetFileName(fileName);
                var filePath = Path.Combine(_photoDirectory, fileName);

                _logger.LogInformation($"Attempting to serve photo: {filePath}");

                if (!System.IO.File.Exists(filePath))
                {
                    _logger.LogWarning($"Photo not found: {filePath}");
                    return NotFound($"Photo {fileName} not found");
                }

                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                var contentType = GetContentType(fileName);

                _logger.LogInformation($"Successfully serving photo: {fileName}, Size: {fileBytes.Length} bytes");

                // Add cache headers for better performance
                Response.Headers.Add("Cache-Control", "public, max-age=3600");
                Response.Headers.Add("ETag", $"\"{fileName}\"");

                return File(fileBytes, contentType, enableRangeProcessing: true);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, $"Access denied when trying to read photo: {fileName}");
                return StatusCode(403, "Access denied to photo");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving photo: {fileName}");
                return StatusCode(500, $"Error retrieving photo: {ex.Message}");
            }
        }

        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                ".svg" => "image/svg+xml",
                _ => "application/octet-stream"
            };
        }
    }
}
