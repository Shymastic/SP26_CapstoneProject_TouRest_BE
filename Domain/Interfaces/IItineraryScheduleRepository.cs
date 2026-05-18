using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;

namespace TouRest.Domain.Interfaces
{
    public interface IItineraryScheduleRepository : IBaseRepository<ItinerarySchedule>
    {
        Task<List<ItinerarySchedule>> GetByItineraryIdAsync(Guid itineraryId);
        Task<ItinerarySchedule?> GetByIdWithGuideAsync(Guid id);
        Task<ItinerarySchedule?> GetScheduleWithDetails(Guid scheduleId);
    }
}
