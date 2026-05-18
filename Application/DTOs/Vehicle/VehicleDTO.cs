using TouRest.Domain.Enums;

namespace TouRest.Application.DTOs.Vehicle
{
    public class VehicleDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int Capacity { get; set; }
        public VehicleType Type { get; set; }
        public Guid AgencyId { get; set; }
    }

    public class VehicleCreateRequest
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int Capacity { get; set; }
        public VehicleType Type { get; set; }
    }

    public class VehicleUpdateRequest
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int Capacity { get; set; }
        public VehicleType Type { get; set; }
    }
}
