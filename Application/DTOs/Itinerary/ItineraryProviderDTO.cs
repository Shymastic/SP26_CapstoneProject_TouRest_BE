namespace TouRest.Application.DTOs.Itinerary
{
    public class ItineraryProviderDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string ContactPhone { get; set; } = null!;
        public List<string> Services { get; set; } = [];
        public List<string> Images { get; set; } = [];
    }
}
