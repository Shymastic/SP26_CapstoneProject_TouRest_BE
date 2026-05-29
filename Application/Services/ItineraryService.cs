using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.Common.Helpers;
using TouRest.Application.DTOs.Itinerary;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class ItineraryService : IItineraryService
    {
        private readonly IItineraryRepository _itineraryRepository;
        private readonly IItineraryStopRepository _stopRepository;
        private readonly IItineraryActivityRepository _activityRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IItineraryScheduleRepository _scheduleRepository;
        private readonly IStorageService _storageService;
        private readonly IMapper _mapper;

        public ItineraryService(
            IItineraryRepository itineraryRepository,
            IItineraryStopRepository stopRepository,
            IItineraryActivityRepository activityRepository,
            IImageRepository imageRepository,
            IItineraryScheduleRepository scheduleRepository,
            IStorageService storageService,
            IMapper mapper)
        {
            _itineraryRepository = itineraryRepository;
            _stopRepository = stopRepository;
            _activityRepository = activityRepository;
            _imageRepository = imageRepository;
            _scheduleRepository = scheduleRepository;
            _storageService = storageService;
            _mapper = mapper;
        }

        public async Task<ItineraryDTO> AddItinerary(Guid agencyId, ItineraryCreateRequest create)
        {
            var itinerary = _mapper.Map<Itinerary>(create);
            itinerary.AgencyId = agencyId;
            itinerary.Status = ItineraryStatus.Draft; 
            itinerary.SpotLeft = create.MaxCapacity;  
            itinerary.CreatedAt = DateTime.UtcNow;
            itinerary.UpdatedAt = DateTime.UtcNow;


            var result = await _itineraryRepository.CreateAsync(itinerary);
            return _mapper.Map<ItineraryDTO>(result);
        }

        public async Task DeleteItinerary(Guid id)
        {
            var itinerary = await _itineraryRepository.GetByIdAsync(id);
            if(itinerary == null)
            {
                throw new KeyNotFoundException("Itinerary not found");
            }
            itinerary.Status = ItineraryStatus.Inactive;
            await _itineraryRepository.UpdateAsync(itinerary);
        }

        public async Task<DTOs.Common.PagedResult<ItineraryDTO>> GetItineraries(ItinerarySearch search)
        {
            var total = search.Limit == null
                ? await _itineraryRepository.CountItineraries(search)
                : 0;

            var list = await _itineraryRepository.GetItineraries(search);
            var dtos = _mapper.Map<List<ItineraryDTO>>(list ?? []);

            foreach (var dto in dtos)
            {
                var images = await _imageRepository.GetByTypeAsync(Domain.Enums.ImageType.Itinerary, dto.Id);
                dto.Images = _mapper.Map<List<DTOs.Image.ImageDTO>>(images);
            }

            return new DTOs.Common.PagedResult<ItineraryDTO>
            {
                Items = dtos,
                Total = search.Limit == null ? total : dtos.Count,
                Page = search.Page,
                PageSize = search.Limit ?? search.PageSize,
            };
        }

        public async Task<ItineraryDTO> UpdateItinerary(Guid id, ItineraryUpdateRequest update)
        {
            var existing = await _itineraryRepository.GetByIdAsync(id);
            if(existing == null)
                throw new KeyNotFoundException("Itinerary not found");
            if (!ItineraryStatusTransitions.CanTransition(existing.Status, update.Status))
                throw new InvalidOperationException(
                    $"Invalid transition from {existing.Status} to {update.Status}");
            existing.UpdatedAt = DateTime.UtcNow;
            _mapper.Map(update, existing);
            existing.Status = update.Status;
            var result = await _itineraryRepository.UpdateAsync(existing);
            return _mapper.Map<ItineraryDTO>(result);
        }
        public async Task<ItineraryDTO?> GetItineraryById(Guid id)
        {
            var itinerary = await _itineraryRepository.GetByIdAsync(id);
            if (itinerary == null)
                return null;
            var dto = _mapper.Map<ItineraryDTO>(itinerary);
            var images = await _imageRepository.GetByTypeAsync(Domain.Enums.ImageType.Itinerary, id);
            dto.Images = _mapper.Map<List<DTOs.Image.ImageDTO>>(images);
            var schedules = await _scheduleRepository.GetByItineraryIdAsync(id);
            dto.Schedules = _mapper.Map<List<ItineraryScheduleDTO>>(
                schedules.Where(s => s.Status == Domain.Enums.ItineraryScheduleStatus.Confirmed).ToList());
            return dto;
        }
        public async Task<ItineraryDTO?> UpdateItineraryStatus(Guid id, ItineraryUpdateStatusRequest request)
        {
            var itinerary = await _itineraryRepository.GetByIdAsync(id);
            if (itinerary == null)
                throw new KeyNotFoundException("Itinerary not found");
            if (!ItineraryStatusTransitions.CanTransition(itinerary.Status, request.Status))
                throw new InvalidOperationException(
                    $"Invalid transition from {itinerary.Status} to {request.Status}");
            itinerary.Status = request.Status;
            itinerary.UpdatedAt = DateTime.UtcNow;
            var result = await _itineraryRepository.UpdateAsync(itinerary);
            return _mapper.Map<ItineraryDTO>(result);
        }

        public async Task<ItineraryDTO> CreateFullAsync(Guid agencyId, ItineraryFullCreateRequest request)
        {
            var itinerary = new Itinerary
            {
                Id = Guid.NewGuid(),
                AgencyId = agencyId,
                Name = request.Name.Trim(),
                Description = request.Description?.Trim(),
                Price = request.Price,
                DurationDays = request.Duration,
                Status = ItineraryStatus.Draft,
                CreatedAt = DateTime.UtcNow,
            };
            var saved = await _itineraryRepository.CreateAsync(itinerary);

            foreach (var stopReq in request.Stops)
            {
                var stop = new ItineraryStop
                {
                    Id = Guid.NewGuid(),
                    ItineraryId = saved.Id,
                    StopOrder = stopReq.StopOrder,
                    Name = stopReq.Name.Trim(),
                    Longitude = stopReq.Longitude,
                    Latitude = stopReq.Latitude,
                    Address = stopReq.Address?.Trim(),
                    ProviderId = stopReq.ProviderId,
                    VehicleId = stopReq.VehicleId,
                    CreatedAt = DateTime.UtcNow,
                };
                var savedStop = await _stopRepository.CreateAsync(stop);

                int actOrder = 0;
                foreach (var actReq in stopReq.Activities)
                {
                    bool isCustom = actReq.ServiceId == null || actReq.ServiceId == Guid.Empty;
                    if (isCustom && string.IsNullOrWhiteSpace(actReq.CustomName)) continue;

                    var activity = new ItineraryActivity
                    {
                        Id = Guid.NewGuid(),
                        ItineraryStopId = savedStop.Id,
                        ServiceId = isCustom ? null : actReq.ServiceId,
                        CustomName = isCustom ? actReq.CustomName!.Trim() : null,
                        ActivityOrder = actReq.ActivityOrder > 0 ? actReq.ActivityOrder : actOrder,
                        StartTime = ParseTime(actReq.StartTime),
                        EndTime = ParseTime(actReq.EndTime),
                        Price = actReq.Price,
                        Note = actReq.Note,
                        CreatedAt = DateTime.UtcNow,
                    };
                    await _activityRepository.CreateAsync(activity);
                    actOrder++;
                }
            }

            if (request.Images != null && request.Images.Count > 0)
            {
                var urls = await _storageService.UploadManyAsync(request.Images);
                for (int i = 0; i < urls.Count; i++)
                {
                    await _imageRepository.CreateAsync(new Image
                    {
                        Id        = Guid.NewGuid(),
                        Url       = urls[i],
                        Type      = ImageType.Itinerary,
                        TypeId    = saved.Id,
                        PicNumber = i + 1,
                    });
                }
            }

            return _mapper.Map<ItineraryDTO>(saved);
        }

        public async Task<List<ItineraryDTO>> GetMyItinerariesAsync(Guid agencyId)
        {
            var list = await _itineraryRepository.GetByAgencyIdAsync(agencyId);
            return _mapper.Map<List<ItineraryDTO>>(list);
        }

        public async Task<List<ItineraryProviderDTO>> GetProvidersInItineraryAsync(Guid itineraryId)
        {
            var stops = await _stopRepository.GetWithProviderAndActivitiesByItineraryIdAsync(itineraryId);
            var seen = new HashSet<Guid>();
            var result = new List<ItineraryProviderDTO>();

            foreach (var stop in stops)
            {
                if (stop.Provider == null || !seen.Add(stop.Provider.Id)) continue;

                var services = stop.Activities
                    .Select(a => a.Service?.Name ?? a.CustomName)
                    .Where(n => !string.IsNullOrWhiteSpace(n))
                    .Select(n => n!)
                    .Distinct()
                    .ToList();

                var images = await _imageRepository.GetByTypeAsync(ImageType.Provider, stop.Provider.Id);
                var imageUrls = images.Select(i => i.Url).ToList();

                result.Add(new ItineraryProviderDTO
                {
                    Id = stop.Provider.Id,
                    Name = stop.Provider.Name,
                    Description = stop.Provider.Description,
                    Address = stop.Provider.Address,
                    ContactPhone = stop.Provider.ContactPhone,
                    Services = services,
                    Images = imageUrls,
                });
            }

            return result;
        }

        private static DateTime ParseTime(string? timeStr)
        {
            if (string.IsNullOrWhiteSpace(timeStr)) return DateTime.UtcNow;
            if (TimeSpan.TryParseExact(timeStr, @"hh\:mm", null, out var ts))
                return DateTime.UtcNow.Date.Add(ts);
            if (DateTime.TryParse(timeStr, out var dt))
                return dt;
            return DateTime.UtcNow;
        }
    }
}
