using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TouRest.Api.Common;
using TouRest.Application.DTOs.Service;
using TouRest.Application.Interfaces;

namespace TouRest.Api.Controllers
{
    [Route("api/services")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly ILogger<ServiceController> _logger;
        private readonly IServiceService _serviceService;
        public ServiceController(ILogger<ServiceController> logger, IServiceService serviceService)
        {
            _logger = logger;
            _serviceService = serviceService;
        }
        [HttpGet("provider/{providerId:guid}")]
        public async Task<IActionResult> GetServicesByProvider(Guid providerId)
        {
            var services = await _serviceService.GetServicesByProviderId(providerId);
            return ApiResponseFactory.Ok(services);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetServiceById(Guid id)
        {
            var service = await _serviceService.GetServiceById(id);
            if (service == null)
                return NotFound();
            return ApiResponseFactory.Ok(service);
        }
        [HttpGet]
        public async Task<IActionResult> GetServices()
        {
            var services = await _serviceService.GetAllServices();
            return ApiResponseFactory.Ok(services);
        }
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateService([FromRoute] Guid id, [FromBody] ServiceUpdateRequest update)
        {
            var result = await _serviceService.UpdateService(id, update);
            return ApiResponseFactory.Ok(result, "Service updated");
        }
        [HttpPost]
        public async Task<IActionResult> AddService([FromBody] ServiceCreateRequest create)
        {
            var result = await _serviceService.CreateService(create);
            return ApiResponseFactory.Created(result, "Service created");
        }
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteService(Guid id)
        {
            var result = await _serviceService.DeleteService(id);
            if (!result)
                return NotFound();
            return ApiResponseFactory.Ok(result, "Service deleted");
        }
    }
    }
