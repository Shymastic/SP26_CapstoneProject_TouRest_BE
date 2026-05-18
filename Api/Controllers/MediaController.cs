using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouRest.Api.Common;
using TouRest.Application.Interfaces;

namespace TouRest.Api.Controllers
{
    [ApiController]
    [Route("api/media")]
    [Authorize]
    public class MediaController : ControllerBase
    {
        private readonly IStorageService _storageService;

        public MediaController(IStorageService storageService)
        {
            _storageService = storageService;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file provided");

            var url = await _storageService.UploadAsync(file);
            return ApiResponseFactory.Ok(new { url });
        }
    }
}
