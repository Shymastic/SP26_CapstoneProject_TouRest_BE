using Microsoft.EntityFrameworkCore;
using TouRest.Domain.DTOs;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;
using TouRest.Infrastructure.Persistence;

namespace TouRest.Infrastructure.Repositories
{
    public class ProviderStaffRepository : IProviderStaffRepository
    {
        private readonly AppDbContext _context;

        public ProviderStaffRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProviderTourGroupDTO>> GetTourGroupsAsync(Guid providerId)
        {
            var validStatuses = new[]
            {
                ItineraryScheduleStatus.Confirmed,
                ItineraryScheduleStatus.Ongoing,
                ItineraryScheduleStatus.Completed
            };

            var schedules = await _context.ItinerarySchedules
                .AsNoTracking()
                .Include(s => s.Itinerary).ThenInclude(i => i.Agency)
                .Include(s => s.Itinerary).ThenInclude(i => i.Stops)
                .Where(s => validStatuses.Contains(s.Status)
                         && s.Itinerary.Stops.Any(st => st.ProviderId == providerId))
                .ToListAsync();

            if (schedules.Count == 0)
                return [];

            var scheduleIds = schedules.Select(s => s.Id).ToList();

            // bookingId → scheduleId map for non-cancelled bookings
            var biRows = await _context.BookingItineraries
                .AsNoTracking()
                .Where(bi => scheduleIds.Contains(bi.ItineraryScheduleId)
                          && bi.Status != BookingItineraryStatus.Cancelled)
                .Select(bi => new { bi.BookingId, bi.ItineraryScheduleId })
                .ToListAsync();

            var bookingIds = biRows.Select(x => x.BookingId).Distinct().ToList();

            var passengerCounts = await _context.BookingPassengers
                .AsNoTracking()
                .Where(bp => bookingIds.Contains(bp.BookingId))
                .GroupBy(bp => bp.BookingId)
                .Select(g => new { BookingId = g.Key, Count = g.Count() })
                .ToListAsync();

            var passengerCountMap = passengerCounts.ToDictionary(x => x.BookingId, x => x.Count);

            var patientCounts = biRows
                .GroupBy(x => x.ItineraryScheduleId)
                .Select(g => new
                {
                    ScheduleId = g.Key,
                    Total = g.Sum(x => passengerCountMap.TryGetValue(x.BookingId, out var c) ? c : 0)
                })
                .ToList();

            var countMap = patientCounts.ToDictionary(x => x.ScheduleId, x => x.Total);

            return schedules.Select(s => new ProviderTourGroupDTO
            {
                ScheduleId      = s.Id,
                AgencyName      = s.Itinerary.Agency.Name,
                TourName        = s.Itinerary.Name,
                TourDescription = s.Itinerary.Description,
                StartTime       = s.StartTime,
                EndTime         = s.EndTime,
                Status          = s.Status.ToString(),
                TotalPatients   = countMap.TryGetValue(s.Id, out var c) ? c : 0,
                SentCount       = 0
            }).ToList();
        }

        public async Task<List<ProviderPatientDTO>> GetPatientsAsync(Guid scheduleId)
        {
            var rows = await _context.BookingItineraries
                .AsNoTracking()
                .Include(bi => bi.Booking).ThenInclude(b => b.User)
                .Where(bi => bi.ItineraryScheduleId == scheduleId
                          && bi.Status != BookingItineraryStatus.Cancelled)
                .ToListAsync();

            return rows.Select(bi => new ProviderPatientDTO
            {
                BookingItineraryId = bi.Id,
                BookingId          = bi.BookingId,
                BookingCode        = bi.Booking.Code,
                FullName           = bi.Booking.User?.FullName,
                Phone              = bi.Booking.User?.Phone,
                DateOfBirth        = bi.Booking.User?.DateOfBirth,
                NumberOfGuests     = bi.NumberOfGuests,
                ResultSent         = false,
                SentAt             = null
            }).ToList();
        }

        public async Task<List<ProviderPassengerDTO>> GetPassengersAsync(Guid scheduleId)
        {
            var rows = await (
                from bp in _context.BookingPassengers
                join bi in _context.BookingItineraries
                    on bp.BookingId equals bi.BookingId
                join b in _context.Bookings
                    on bi.BookingId equals b.Id
                where bi.ItineraryScheduleId == scheduleId
                select new { bp, b.Code }
            ).AsNoTracking().Distinct().ToListAsync();

            if (rows.Count == 0) return [];

            var passengerIds = rows.Select(r => r.bp.Id).ToList();

            var resultMap = await _context.MedicalResults
                .AsNoTracking()
                .Where(mr => mr.ScheduleId == scheduleId && passengerIds.Contains(mr.PassengerId))
                .Select(mr => new { mr.PassengerId, mr.SentAt })
                .ToDictionaryAsync(mr => mr.PassengerId, mr => mr.SentAt);

            return rows.Select(r => new ProviderPassengerDTO
            {
                PassengerId = r.bp.Id,
                BookingId   = r.bp.BookingId,
                BookingCode = r.Code,
                FullName    = r.bp.FullName,
                IdNumber    = r.bp.IdNumber,
                Phone       = r.bp.Phone,
                Age         = r.bp.Age,
                ResultSent  = resultMap.ContainsKey(r.bp.Id),
                SentAt      = resultMap.TryGetValue(r.bp.Id, out var sentAt) ? sentAt : null,
            }).ToList();
        }

        public async Task<ProviderPassengerDTO> SendMedicalResultAsync(
            Guid passengerId, Guid scheduleId, Guid providerId, string? notes, List<string> imageUrls)
        {
            var existing = await _context.MedicalResults
                .Include(mr => mr.Images)
                .FirstOrDefaultAsync(mr => mr.PassengerId == passengerId && mr.ScheduleId == scheduleId);

            if (existing != null)
            {
                existing.Notes     = notes;
                existing.SentAt    = DateTime.UtcNow;
                existing.UpdatedAt = DateTime.UtcNow;
                _context.MedicalResultImages.RemoveRange(existing.Images);
                existing.Images = imageUrls.Select(url => new MedicalResultImage
                {
                    Id              = Guid.NewGuid(),
                    MedicalResultId = existing.Id,
                    ImageUrl        = url,
                    CreatedAt       = DateTime.UtcNow,
                }).ToList();
            }
            else
            {
                var result = new MedicalResult
                {
                    Id          = Guid.NewGuid(),
                    PassengerId = passengerId,
                    ScheduleId  = scheduleId,
                    ProviderId  = providerId,
                    Notes       = notes,
                    SentAt      = DateTime.UtcNow,
                    CreatedAt   = DateTime.UtcNow,
                    Images      = imageUrls.Select(url => new MedicalResultImage
                    {
                        Id       = Guid.NewGuid(),
                        ImageUrl = url,
                        CreatedAt = DateTime.UtcNow,
                    }).ToList(),
                };
                _context.MedicalResults.Add(result);
            }

            await _context.SaveChangesAsync();

            var bp = await _context.BookingPassengers
                .AsNoTracking()
                .Include(p => p.Booking)
                .FirstAsync(p => p.Id == passengerId);

            var medResult = await _context.MedicalResults
                .AsNoTracking()
                .FirstAsync(mr => mr.PassengerId == passengerId && mr.ScheduleId == scheduleId);

            // Notify the booking owner that their passenger's result is ready
            _context.Notifications.Add(new Notification
            {
                Id              = Guid.NewGuid(),
                RecipientUserId = bp.Booking.UserId,
                Title           = "Kết quả khám bệnh đã có",
                Message         = $"Kết quả khám của {bp.FullName} đã được gửi bởi nhà cung cấp dịch vụ.",
                EntityType      = NotificationEntityType.Other,
                EntityId        = medResult.Id,
                IsRead          = false,
                CreatedAt       = DateTime.UtcNow,
            });
            await _context.SaveChangesAsync();

            return new ProviderPassengerDTO
            {
                PassengerId = bp.Id,
                BookingId   = bp.BookingId,
                BookingCode = bp.Booking.Code,
                FullName    = bp.FullName,
                IdNumber    = bp.IdNumber,
                Phone       = bp.Phone,
                Age         = bp.Age,
                ResultSent  = true,
                SentAt      = medResult.SentAt,
            };
        }

        public async Task<BookingStopMedicalResultDTO> GetBookingStopResultsAsync(Guid bookingId, Guid stopId)
        {
            // stop → providerId, providerName, stopName
            var stop = await _context.ItineraryStops
                .AsNoTracking()
                .Include(s => s.Provider)
                .FirstOrDefaultAsync(s => s.Id == stopId)
                ?? throw new KeyNotFoundException($"Stop {stopId} not found.");

            // booking → scheduleId
            var bi = await _context.BookingItineraries
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BookingId == bookingId && b.Status != BookingItineraryStatus.Cancelled)
                ?? throw new KeyNotFoundException($"No active booking itinerary for booking {bookingId}.");

            var scheduleId = bi.ItineraryScheduleId;

            // all passengers for this booking
            var passengers = await _context.BookingPassengers
                .AsNoTracking()
                .Where(bp => bp.BookingId == bookingId)
                .ToListAsync();

            // results sent for these passengers in this schedule
            var passengerIds = passengers.Select(p => p.Id).ToList();
            var results = await _context.MedicalResults
                .AsNoTracking()
                .Include(mr => mr.Images)
                .Where(mr => mr.ScheduleId == scheduleId && passengerIds.Contains(mr.PassengerId))
                .ToListAsync();

            var resultMap = results.ToDictionary(r => r.PassengerId);

            return new BookingStopMedicalResultDTO
            {
                ProviderName = stop.Provider?.Name ?? "Unknown Provider",
                StopName     = stop.Name,
                Passengers   = passengers.Select(p =>
                {
                    resultMap.TryGetValue(p.Id, out var r);
                    return new PassengerMedicalResultDTO
                    {
                        PassengerId = p.Id,
                        FullName    = p.FullName,
                        Age         = p.Age,
                        IdNumber    = p.IdNumber,
                        ResultSent  = r != null,
                        SentAt      = r?.SentAt,
                        Notes       = r?.Notes,
                        ImageUrls   = r?.Images.Select(i => i.ImageUrl).ToList() ?? [],
                    };
                }).ToList(),
            };
        }
    }
}
