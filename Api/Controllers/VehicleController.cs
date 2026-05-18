using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouRest.Api.Common;
using TouRest.Api.Extensions;
using TouRest.Application.DTOs.Vehicle;
using TouRest.Application.Interfaces;

namespace TouRest.Api.Controllers
{
    [Route("api/vehicles")]
    [ApiController]
    [Authorize(Roles = "AGENCY")]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;
        private readonly IAgencyService _agencyService;

        public VehicleController(IVehicleService vehicleService, IAgencyService agencyService)
        {
            _vehicleService = vehicleService;
            _agencyService = agencyService;
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyVehicles()
        {
            var userId = User.GetUserId();
            var agency = await _agencyService.GetMyAgency(userId);
            var vehicles = await _vehicleService.GetMyVehiclesAsync(agency.Id);
            return ApiResponseFactory.Ok(vehicles);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VehicleCreateRequest request)
        {
            var userId = User.GetUserId();
            var agency = await _agencyService.GetMyAgency(userId);
            var result = await _vehicleService.CreateAsync(agency.Id, request);
            return ApiResponseFactory.Created(result, "Vehicle created");
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] VehicleUpdateRequest request)
        {
            var userId = User.GetUserId();
            var agency = await _agencyService.GetMyAgency(userId);
            var result = await _vehicleService.UpdateAsync(id, agency.Id, request);
            return ApiResponseFactory.Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = User.GetUserId();
            var agency = await _agencyService.GetMyAgency(userId);
            await _vehicleService.DeleteAsync(id, agency.Id);
            return ApiResponseFactory.Ok(new { }, "Vehicle deleted");
        }
    }
}
