using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouRest.Api.Common;
using TouRest.Application.DTOs.Package;
using TouRest.Application.Interfaces;

namespace TouRest.Api.Controllers
{
    [ApiController]
    [Route("api/packages")]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _packageService;

        public PackageController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _packageService.GetAllAsync();
            return ApiResponseFactory.Ok(result);
        }

        [HttpGet("provider/{providerId:guid}")]
        public async Task<IActionResult> GetByProvider(Guid providerId)
        {
            var result = await _packageService.GetByProviderIdAsync(providerId);
            return ApiResponseFactory.Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _packageService.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = "Package not found." });
            return ApiResponseFactory.Ok(result);
        }

        [HttpGet("{id:guid}/detail")]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            var result = await _packageService.GetDetailByIdAsync(id);
            if (result == null)
                return NotFound(new { message = "Package not found." });
            return ApiResponseFactory.Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN,PROVIDER")]
        public async Task<IActionResult> Create([FromBody] PackageCreateRequest request)
        {
            var result = await _packageService.CreateAsync(request);
            return ApiResponseFactory.Created(result, "Package created successfully");
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "ADMIN,PROVIDER")]
        public async Task<IActionResult> Update(Guid id, [FromBody] PackageUpdateRequest request)
        {
            var result = await _packageService.UpdateAsync(id, request);
            if (result == null)
            {
                return NotFound(new { message = "Package not found." });
            }

            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "ADMIN,PROVIDER")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _packageService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound(new { message = "Package not found." });
            }

            return NoContent();
        }
    }
}