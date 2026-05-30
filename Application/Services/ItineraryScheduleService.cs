using TouRest.Application.DTOs.Itinerary;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class ItineraryScheduleService : IItineraryScheduleService
    {
        private readonly IItineraryScheduleRepository _repo;
        private readonly IWalletRepository _walletRepository;
        private readonly IWalletTransactionRepository _walletTransactionRepository;
        private readonly IBookingItineraryRepository _bookingItineraryRepository;
        private readonly INotificationRepository      _notificationRepo;
        private readonly IAgencyUserRepository  _agencyUserRepo;

        public ItineraryScheduleService(
            IItineraryScheduleRepository repo,
            IWalletRepository walletRepository,
            IWalletTransactionRepository walletTransactionRepository,
            IBookingItineraryRepository bookingItineraryRepository,
            INotificationRepository notificationRepo,
            IAgencyUserRepository agencyUserRepo
        )
        {
            _repo = repo;
            _walletRepository = walletRepository;
            _walletTransactionRepository = walletTransactionRepository;
            _bookingItineraryRepository = bookingItineraryRepository;
            _notificationRepo = notificationRepo;
            _agencyUserRepo   = agencyUserRepo;
        }

        public async Task<List<ItineraryScheduleDTO>> GetByItineraryIdAsync(Guid itineraryId)
        {
            var list = await _repo.GetByItineraryIdAsync(itineraryId);
            return list.Select(MapToDTO).ToList();
        }

        public async Task<ItineraryScheduleDTO> AddAsync(Guid itineraryId, ItineraryScheduleCreateRequest request)
        {
            if (request.EndTime <= request.StartTime)
                throw new ArgumentException("EndTime must be after StartTime.");
            if (request.Spot < 1)
                throw new ArgumentException("Spot must be at least 1.");

            var schedule = new ItinerarySchedule
            {
                Id          = Guid.NewGuid(),
                ItineraryId = itineraryId,
                StartTime   = request.StartTime,
                EndTime     = request.EndTime,
                Spot        = request.Spot,
                SpotLeft    = request.Spot,
                GuideId     = request.GuideId,
                Status      = ItineraryScheduleStatus.Pending,
                CreatedAt   = DateTime.UtcNow,
            };
            var saved = await _repo.CreateAsync(schedule);

            // Load full details for notification + return value
            var full = await _repo.GetScheduleWithDetails(saved.Id);

            // Notify guide if assigned
            if (request.GuideId.HasValue && full?.Itinerary != null)
            {
                var itineraryName = full.Itinerary.Name;
                var startDate     = request.StartTime.ToString("dd MMM yyyy");
                var endDate       = request.EndTime.ToString("dd MMM yyyy");

                await _notificationRepo.CreateAsync(new Notification
                {
                    RecipientUserId = request.GuideId.Value,
                    Title           = "New Job Assignment",
                    Message         = $"You have been assigned as tour guide for \"{itineraryName}\" ({startDate} – {endDate}). Please go to your Jobs page to accept or reject.",
                    EntityType      = NotificationEntityType.Itinerary,
                    EntityId        = saved.Id,
                });
            }

            return MapToDTO(full ?? saved);
        }

        private static ItineraryScheduleDTO MapToDTO(ItinerarySchedule s) => new()
        {
            Id          = s.Id,
            ItineraryId = s.ItineraryId,
            StartTime   = s.StartTime,
            EndTime     = s.EndTime,
            Spot        = s.Spot,
            SpotLeft    = s.SpotLeft,
            GuideId     = s.GuideId,
            GuideName   = s.Guide != null ? (s.Guide.FullName ?? s.Guide.Username) : null,
        };

        private static AgencyScheduleDTO MapToAgencyDTO(ItinerarySchedule s) => new()
        {
            Id            = s.Id,
            ItineraryId   = s.ItineraryId,
            ItineraryName = s.Itinerary?.Name ?? string.Empty,
            StartTime     = s.StartTime,
            EndTime       = s.EndTime,
            Spot          = s.Spot,
            SpotLeft      = s.SpotLeft,
            GuideId       = s.GuideId,
            GuideName     = s.Guide != null ? (s.Guide.FullName ?? s.Guide.Username) : null,
            Status        = s.Status.ToString(),
        };

        public async Task<List<AgencyScheduleDTO>> GetByAgencyIdAsync(Guid agencyId)
        {
            var list = await _repo.GetByAgencyIdAsync(agencyId);
            return list.Select(MapToAgencyDTO).ToList();
        }

        public async Task<List<AgencyScheduleDTO>> GetByGuideIdAsync(Guid guideId)
        {
            var list = await _repo.GetByGuideIdAsync(guideId);
            return list.Select(MapToAgencyDTO).ToList();
        }

        public async Task<List<ProviderScheduleDTO>> GetByProviderIdAsync(Guid providerId)
        {
            var list = await _repo.GetByProviderIdAsync(providerId);
            return list.Select(s =>
            {
                var firstActivity = s.Itinerary?.Stops
                    .Where(st => st.ProviderId == providerId)
                    .SelectMany(st => st.Activities)
                    .OrderBy(a => a.StartTime)
                    .FirstOrDefault();

                return new ProviderScheduleDTO
                {
                    Id                = s.Id,
                    ItineraryId       = s.ItineraryId,
                    ItineraryName     = s.Itinerary?.Name ?? string.Empty,
                    AgencyName        = s.Itinerary?.Agency?.Name ?? string.Empty,
                    StartTime         = s.StartTime,
                    EndTime           = s.EndTime,
                    Spot              = s.Spot,
                    SpotLeft          = s.SpotLeft,
                    GuideId           = s.GuideId,
                    GuideName         = s.Guide != null ? (s.Guide.FullName ?? s.Guide.Username) : null,
                    FirstActivityTime = firstActivity?.StartTime,
                };
            }).ToList();
        }

        public async Task<bool> DeleteAsync(Guid scheduleId)
        {
            return await _repo.DeleteAsync(scheduleId);
        }

        public async Task AcceptScheduleAsync(Guid scheduleId, Guid guideId)
        {
            var schedule = await _repo.GetByIdAsync(scheduleId)
                ?? throw new KeyNotFoundException("Schedule not found");

            if (schedule.GuideId != guideId)
                throw new UnauthorizedAccessException("You are not assigned to this schedule");

            if (schedule.Status != ItineraryScheduleStatus.Pending)
                throw new InvalidOperationException("Only pending schedules can be accepted");

            schedule.Status    = ItineraryScheduleStatus.Confirmed;
            schedule.UpdatedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(schedule);

            // Notify all agency managers
            await NotifyManagersAsync(scheduleId,
                title:   "Schedule Confirmed",
                message: s => $"Your guide has accepted the schedule for \"{s.Itinerary?.Name}\". The tour is now confirmed.");
        }

        public async Task RejectScheduleAsync(Guid scheduleId, Guid guideId)
        {
            var schedule = await _repo.GetByIdAsync(scheduleId)
                ?? throw new KeyNotFoundException("Schedule not found");

            if (schedule.GuideId != guideId)
                throw new UnauthorizedAccessException("You are not assigned to this schedule");

            if (schedule.Status != ItineraryScheduleStatus.Pending)
                throw new InvalidOperationException("Only pending schedules can be rejected");

            schedule.GuideId   = null;
            schedule.Status    = ItineraryScheduleStatus.Pending;
            schedule.UpdatedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(schedule);

            // Notify all agency managers
            await NotifyManagersAsync(scheduleId,
                title:   "Schedule Rejected",
                message: s => $"Your guide has declined the schedule for \"{s.Itinerary?.Name}\". Please assign a new guide.");
        }

        private async Task NotifyManagersAsync(Guid scheduleId, string title, Func<ItinerarySchedule, string> message)
        {
            var details = await _repo.GetScheduleWithDetails(scheduleId);
            if (details?.Itinerary == null) return;

            var agencyUsers = await _agencyUserRepo.GetAgencyUsers(details.Itinerary.AgencyId);
            var managers    = agencyUsers.Where(u => u.Role == AgencyUserRole.Manager);

            foreach (var manager in managers)
            {
                await _notificationRepo.CreateAsync(new Notification
                {
                    RecipientUserId = manager.UserId,
                    Title           = title,
                    Message         = message(details),
                    EntityType      = NotificationEntityType.Itinerary,
                    EntityId        = scheduleId,
                });
            }
        }
        public async Task UpdateStatusAsync(Guid scheduleId, ItineraryScheduleStatus status)
        {
            var schedule = await _repo.GetWithStopsAndActivitiesAsync(scheduleId);
            if (schedule == null) throw new KeyNotFoundException("Schedule not found");

            schedule.Status = status;
            schedule.UpdatedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(schedule);

            if (status != ItineraryScheduleStatus.Completed) return;

            // Credit providers
            var providerEarnings = schedule.Itinerary.Stops
                .Where(s => s.ProviderId.HasValue)
                .GroupBy(s => s.ProviderId!.Value)
                .Select(g => new
                {
                    ProviderId = g.Key,
                    Total = g.SelectMany(s => s.Activities).Sum(a => a.Price)
                });

            foreach (var earning in providerEarnings)
            {
                var wallet = await _walletRepository.GetByProviderIdAsync(earning.ProviderId);
                if (wallet == null) continue;

                wallet.Balance += earning.Total;
                wallet.UpdatedAt = DateTime.UtcNow;
                await _walletRepository.UpdateAsync(wallet);

                await _walletTransactionRepository.CreateAsync(new WalletTransaction
                {
                    Id = Guid.NewGuid(),
                    WalletId = wallet.Id,
                    Amount = earning.Total,
                    Type = WalletTransactionType.Credit,
                    Reason = WalletTransactionReason.BookingEarning,
                    ReferenceId = scheduleId,
                    Note = $"Provider earnings from schedule {scheduleId}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            var bookingItineraries = await _bookingItineraryRepository.GetByScheduleIdAsync(scheduleId);
            var agencyGroups = bookingItineraries.GroupBy(bi => bi.ItinerarySchedule.Itinerary.AgencyId);

            foreach (var group in agencyGroups)
            {
                var wallet = await _walletRepository.GetByAgencyIdAsync(group.Key);
                if (wallet == null) continue;

                var total = group.Sum(bi => bi.FinalPrice);
                wallet.Balance += total;
                wallet.UpdatedAt = DateTime.UtcNow;
                await _walletRepository.UpdateAsync(wallet);

                await _walletTransactionRepository.CreateAsync(new WalletTransaction
                {
                    Id = Guid.NewGuid(),
                    WalletId = wallet.Id,
                    Amount = total,
                    Type = WalletTransactionType.Credit,
                    Reason = WalletTransactionReason.BookingEarning,
                    ReferenceId = scheduleId,
                    Note = $"Agency earnings from schedule {scheduleId}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }

    }
}
