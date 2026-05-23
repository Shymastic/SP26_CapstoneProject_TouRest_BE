using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;

namespace TouRest.Domain.Interfaces
{
    public interface IItineraryStopRepository : IBaseRepository<ItineraryStop>
    {
        Task<List<ItineraryStop>> GetByItineraryIdAsync(Guid itineraryId);
        Task<List<ItineraryStop>> GetWithActivitiesByItineraryIdAsync(Guid itineraryId);
        Task<List<ItineraryStop>> GetWithProviderAndActivitiesByItineraryIdAsync(Guid itineraryId);
        Task<ItineraryStop?> GetItineraryStop(Guid id);
        Task UpdateRangeAsync(List<ItineraryStop> ordered);
    }
}
