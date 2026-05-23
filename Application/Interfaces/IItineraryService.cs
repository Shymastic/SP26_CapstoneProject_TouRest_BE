using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Common;
using TouRest.Application.DTOs.Itinerary;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Interfaces
{
    public interface IItineraryService
    {
        Task<PagedResult<ItineraryDTO>> GetItineraries(ItinerarySearch search);
        Task<ItineraryDTO> AddItinerary(Guid agencyId, ItineraryCreateRequest create);
        Task<ItineraryDTO> UpdateItinerary(Guid id, ItineraryUpdateRequest update);
        Task DeleteItinerary(Guid id);
        Task<ItineraryDTO?> GetItineraryById(Guid id);
        Task<ItineraryDTO?> UpdateItineraryStatus(Guid id, ItineraryUpdateStatusRequest status);
        Task<ItineraryDTO> CreateFullAsync(Guid agencyId, ItineraryFullCreateRequest request);
        Task<List<ItineraryDTO>> GetMyItinerariesAsync(Guid agencyId);
        Task<List<ItineraryProviderDTO>> GetProvidersInItineraryAsync(Guid itineraryId);
    }
}
