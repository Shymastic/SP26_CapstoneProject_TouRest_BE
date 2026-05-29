using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;

namespace TouRest.Domain.Interfaces
{
    public interface IItineraryRepository : IBaseRepository<Itinerary>
    {
        Task<List<Itinerary>> GetItineraries(ItinerarySearch search);
        Task<int> CountItineraries(ItinerarySearch search);
        Task<List<Itinerary>> GetByAgencyIdAsync(Guid agencyId);
        Task<int> CountActiveByAgencyIdAsync(Guid agencyId);
    }
    public class ItinerarySearch
    {
        public Guid? AgencyId { get; set; }
        public string? AgencyName { get; set; }
        public string? Name { get; set; }
        public long? LowPrice { get; set; }
        public long? HighPrice { get; set; }
        public int? LowDurationDay { get; set; }
        public int? HighDurationDay { get; set; }
        public string? Status { get; set; }
        public int? Limit { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public string? Destination { get; set; }
        public VehicleType? VehicleType { get; set; }
    }
}
