using AutoMapper;
using TouRest.Application.DTOs.Booking;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly IBookingItineraryRepository _bookingItineraryRepo;
        private readonly IBookingPassengerRepository _passengerRepo;
        private readonly IItineraryScheduleRepository _scheduleRepo;
        private readonly IItineraryRepository _itineraryRepo;
        private readonly IVoucherRepository _voucherRepo;
        private readonly INotificationRepository _notificationRepo;
        private readonly IMapper _mapper;

        public BookingService(
            IBookingRepository bookingRepo,
            IBookingItineraryRepository bookingItineraryRepo,
            IBookingPassengerRepository passengerRepo,
            IItineraryScheduleRepository scheduleRepo,
            IItineraryRepository itineraryRepo,
            IVoucherRepository voucherRepo,
            INotificationRepository notificationRepo,
            IMapper mapper)
        {
            _bookingRepo          = bookingRepo;
            _bookingItineraryRepo = bookingItineraryRepo;
            _passengerRepo        = passengerRepo;
            _scheduleRepo         = scheduleRepo;
            _itineraryRepo        = itineraryRepo;
            _voucherRepo          = voucherRepo;
            _notificationRepo     = notificationRepo;
            _mapper               = mapper;
        }

        public async Task<BookingDTO> GetBookingAsync(Guid id, Guid userId, bool isAdmin)
        {
            var booking = await _bookingRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Booking not found");
            if (!isAdmin && booking.UserId != userId)
                throw new UnauthorizedAccessException("You do not own this booking");
            return _mapper.Map<BookingDTO>(booking);
        }

        public async Task<BookingCreateResponse> CreateBookingAsync(BookingCreateRequest request, Guid userId)
        {
            // 1. Load schedule and check spots
            var schedule = await _scheduleRepo.GetByIdAsync(request.ScheduleId)
                ?? throw new KeyNotFoundException("Schedule not found");

            if (schedule.SpotLeft < request.NumberOfGuests)
                throw new InvalidOperationException(
                    $"Not enough spots. Available: {schedule.SpotLeft}, requested: {request.NumberOfGuests}");

            // 2. Load itinerary for base price
            var itinerary = await _itineraryRepo.GetByIdAsync(schedule.ItineraryId)
                ?? throw new KeyNotFoundException("Itinerary not found");

            long baseAmount = itinerary.Price * request.NumberOfGuests;

            // 3. Validate voucher
            var (voucher, voucherError) = await ValidateVoucherAsync(request.VoucherCode, baseAmount);
            if (voucherError != null)
                throw new ArgumentException(voucherError);

            // 4. Apply discount
            long discountAmount = CalculateDiscount(baseAmount, voucher);
            long totalAmount    = baseAmount - discountAmount;

            // 5. Create booking
            var code = GenerateCode();
            var booking = new Booking
            {
                Id            = Guid.NewGuid(),
                UserId        = userId,
                Code          = code,
                TotalAmount   = totalAmount,
                Status        = BookingStatus.Pending,
                PaymentStatus = PaymentStatus.Pending,
                CustomerNote  = request.CustomerNote,
                CreatedAt     = DateTime.UtcNow,
                UpdatedAt     = DateTime.UtcNow,
            };
            await _bookingRepo.CreateAsync(booking);

            // 6. Create booking itinerary line
            var line = new BookingItinerary
            {
                Id                  = Guid.NewGuid(),
                BookingId           = booking.Id,
                ItineraryScheduleId = schedule.Id,
                VoucherId           = voucher?.Id,
                Price               = baseAmount,   // total gross (unit price × guests)
                FinalPrice          = totalAmount,  // total net (after discount)
                NumberOfGuests      = request.NumberOfGuests,
                Status              = BookingItineraryStatus.Pending,
                CreatedAt           = DateTime.UtcNow,
                UpdatedAt           = DateTime.UtcNow,
            };
            await _bookingItineraryRepo.CreateAsync(line);

            // 7. Save passengers
            foreach (var p in request.Passengers)
            {
                var passenger = _mapper.Map<BookingPassenger>(p);
                passenger.BookingId = booking.Id;
                await _passengerRepo.CreateAsync(passenger);
            }

            // 8. Decrease schedule spots
            schedule.SpotLeft -= request.NumberOfGuests;
            await _scheduleRepo.UpdateAsync(schedule);

            // 8. Increment voucher usage
            if (voucher != null)
            {
                voucher.UsedCount++;
                await _voucherRepo.UpdateAsync(voucher);
            }

            // 9. Create notification for the customer
            await _notificationRepo.CreateAsync(new Notification
            {
                Id              = Guid.NewGuid(),
                RecipientUserId = userId,
                Title           = "Booking Created",
                Message         = $"Your booking #{code} has been created. Please complete payment to confirm.",
                EntityType      = NotificationEntityType.Booking,
                EntityId        = booking.Id,
                IsRead          = false,
                CreatedAt       = DateTime.UtcNow,
            });

            return new BookingCreateResponse
            {
                BookingId      = booking.Id,
                Code           = code,
                BaseAmount     = baseAmount,
                DiscountAmount = discountAmount,
                TotalAmount    = totalAmount,
                VoucherApplied = voucher?.Code,
            };
        }

        public async Task<BookingDTO> UpdateBookingAsync(Guid id, Guid userId, bool isAdmin, BookingUpdateRequest request)
        {
            var existing = await _bookingRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Booking not found");
            if (!isAdmin && existing.UserId != userId)
                throw new UnauthorizedAccessException("You do not own this booking");
            _mapper.Map(request, existing);
            existing.UpdatedAt = DateTime.UtcNow;
            var updated = await _bookingRepo.UpdateAsync(existing);
            return _mapper.Map<BookingDTO>(updated);
        }

        public async Task DeleteBookingAsync(Guid id, Guid userId, bool isAdmin)
        {
            var existing = await _bookingRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Booking not found");
            if (!isAdmin && existing.UserId != userId)
                throw new UnauthorizedAccessException("You do not own this booking");
            existing.Status    = BookingStatus.Cancelled;
            existing.UpdatedAt = DateTime.UtcNow;
            await _bookingRepo.UpdateAsync(existing);
        }

        public async Task<List<BookingDTO>> GetBookingsByUserIdAsync(Guid userId, string? status = null)
        {
            BookingStatus? parsedStatus = null;
            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<BookingStatus>(status, true, out var parsed))
                parsedStatus = parsed;
            var bookings = await _bookingRepo.GetBookingsByUserIdAsync(userId, parsedStatus);
            return _mapper.Map<List<BookingDTO>>(bookings);
        }

        // ── Private helpers ───────────────────────────────────────────────────

        private async Task<(Voucher? voucher, string? error)> ValidateVoucherAsync(string? code, long baseAmount)
        {
            if (string.IsNullOrWhiteSpace(code)) return (null, null);

            var voucher = await _voucherRepo.GetByCodeAsync(code);
            if (voucher == null)
                return (null, "Voucher không tồn tại");

            if (voucher.Status != VoucherStatus.Active)
                return (null, "Voucher không còn hiệu lực");

            var now = DateTime.UtcNow;
            if (now < voucher.ValidFrom || now > voucher.ValidTo)
                return (null, "Voucher đã hết hạn");

            if (voucher.UsageLimit.HasValue && voucher.UsedCount >= voucher.UsageLimit.Value)
                return (null, "Voucher đã hết lượt sử dụng");

            if (voucher.MinOrderAmount.HasValue && baseAmount < (long)voucher.MinOrderAmount.Value)
                return (null, $"Đơn hàng tối thiểu {voucher.MinOrderAmount.Value:N0}đ để dùng voucher này");

            return (voucher, null);
        }

        private static long CalculateDiscount(long baseAmount, Voucher? voucher)
        {
            if (voucher == null) return 0;

            long discount = voucher.DiscountType == DiscountType.Percent
                ? (long)(baseAmount * voucher.DiscountValue / 100.0)
                : voucher.DiscountValue;

            if (voucher.MaxDiscountAmount.HasValue)
                discount = Math.Min(discount, (long)voucher.MaxDiscountAmount.Value);

            return Math.Min(discount, baseAmount);
        }

        private static string GenerateCode() =>
            $"BK-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
    }
}
