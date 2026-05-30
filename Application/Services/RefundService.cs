using AutoMapper;
using TouRest.Application.DTOs.Refund;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class RefundService : IRefundService
    {
        private readonly IRefundRepository _refundRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IBookingItineraryRepository _bookingItineraryRepo;
        private readonly IItineraryScheduleRepository _scheduleRepo;
        private readonly IWalletRepository _walletRepo;
        private readonly IWalletTransactionRepository _walletTxRepo;
        private readonly INotificationRepository _notificationRepo;
        private readonly IMapper _mapper;

        public RefundService(
            IRefundRepository refundRepository,
            IPaymentRepository paymentRepository,
            IBookingRepository bookingRepository,
            IBookingItineraryRepository bookingItineraryRepo,
            IItineraryScheduleRepository scheduleRepo,
            IWalletRepository walletRepo,
            IWalletTransactionRepository walletTxRepo,
            INotificationRepository notificationRepo,
            IMapper mapper)
        {
            _refundRepository     = refundRepository;
            _paymentRepository    = paymentRepository;
            _bookingRepository    = bookingRepository;
            _bookingItineraryRepo = bookingItineraryRepo;
            _scheduleRepo         = scheduleRepo;
            _walletRepo           = walletRepo;
            _walletTxRepo         = walletTxRepo;
            _notificationRepo     = notificationRepo;
            _mapper               = mapper;
        }

        public async Task<RefundDTO> RequestRefundAsync(RefundRequestDTO request, Guid userId)
        {
            var booking = await _bookingRepository.GetByIdAsync(request.BookingId);
            if (booking == null)
                throw new KeyNotFoundException("Booking not found");
            if (booking.UserId != userId)
                throw new UnauthorizedAccessException("You do not own this booking");
            if (booking.Status != BookingStatus.Confirmed)
                throw new InvalidOperationException("Only confirmed bookings can be refunded");

            var payment = await _paymentRepository.GetActivePaymentByBookingIdAsync(request.BookingId);
            if (payment == null || payment.Status != PaymentStatus.Paid)
                throw new InvalidOperationException("No completed payment found for this booking");

            var existing = await _refundRepository.GetByPaymentIdAsync(payment.Id);
            if (existing != null)
                throw new InvalidOperationException("A refund already exists for this payment");

            var refund = new Refund
            {
                Id = Guid.NewGuid(),
                BookingId = request.BookingId,
                PaymentId = payment.Id,
                TotalRefundAmount = payment.FinalAmount,
                InitiatedBy = RefundInitinator.Customer,
                Reason = request.Reason,
                CustomerBankAccount = request.CustomerBankAccount,
                CustomerBankName = request.CustomerBankName,
                CustomerAccountHolder = request.CustomerAccountHolder,
                Status = RefundStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var created = await _refundRepository.CreateAsync(refund);
            return _mapper.Map<RefundDTO>(created);
        }

        public async Task<RefundDTO> ReviewRefundAsync(Guid refundId, RefundReviewDTO review)
        {
            var refund = await CheckRefund(refundId);
            if (refund.Status != RefundStatus.Pending)
                throw new InvalidOperationException("Refund is not pending");

            refund.Status = review.Approved ? RefundStatus.Approved : RefundStatus.Rejected;
            refund.AdminNote = review.AdminNote;
            refund.UpdatedAt = DateTime.UtcNow;

            var updated = await _refundRepository.UpdateAsync(refund);
            return _mapper.Map<RefundDTO>(updated);
        }

        public async Task<RefundDTO> CompleteRefundAsync(Guid refundId)
        {
            var refund = await CheckRefund(refundId);
            if (refund.Status != RefundStatus.Approved)
                throw new InvalidOperationException("Refund must be approved before completing");

            refund.Status = RefundStatus.Completed;
            refund.RefundedAt = DateTime.UtcNow;
            refund.UpdatedAt = DateTime.UtcNow;

            // Cancel the booking
            var booking = await _bookingRepository.GetByIdAsync(refund.BookingId);
            if (booking != null)
            {
                booking.Status = BookingStatus.Cancelled;
                booking.UpdatedAt = DateTime.UtcNow;
                await _bookingRepository.UpdateAsync(booking);
            }

            var updated = await _refundRepository.UpdateAsync(refund);
            return _mapper.Map<RefundDTO>(updated);
        }

        public async Task<RefundDTO> GetRefundByBookingAsync(Guid bookingId)
        {
            var refund = await _refundRepository.GetByBookingIdAsync(bookingId);
            if (refund == null)
                throw new KeyNotFoundException("Refund not found");
            return _mapper.Map<RefundDTO>(refund);
        }

        public async Task<CancelBookingResultDTO> CancelAndRefundAsync(Guid bookingId, Guid userId, string? reason)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId)
                ?? throw new KeyNotFoundException("Booking not found");

            if (booking.UserId != userId)
                throw new UnauthorizedAccessException("You do not own this booking");

            if (booking.Status == BookingStatus.Cancelled)
                throw new InvalidOperationException("Booking is already cancelled");

            // Get departure date via BookingItinerary → ItinerarySchedule
            var lines = await _bookingItineraryRepo.GetBookingItinerariesByBookingId(bookingId);
            var line  = lines.FirstOrDefault(l => l.Status != BookingItineraryStatus.Cancelled);

            DateTime? departureDate = null;
            if (line != null)
            {
                var schedule = await _scheduleRepo.GetByIdAsync(line.ItineraryScheduleId);
                departureDate = schedule?.StartTime;
            }

            // Determine refund %
            int refundPercent = 0;
            if (departureDate.HasValue)
            {
                var daysUntil = (departureDate.Value.Date - DateTime.UtcNow.Date).TotalDays;
                refundPercent = daysUntil >= 7 ? 100
                              : daysUntil >= 2 ? 50
                              : 0;
            }

            // Get paid amount (only refund if payment exists)
            var payment = await _paymentRepository.GetActivePaymentByBookingIdAsync(bookingId);
            long paidAmount   = payment?.FinalAmount ?? 0;
            long refundAmount = refundPercent > 0 && paidAmount > 0
                ? paidAmount * refundPercent / 100
                : 0;

            // Credit wallet if there's something to refund
            if (refundAmount > 0)
            {
                var wallet = await _walletRepo.GetByUserIdAsync(userId);
                if (wallet == null)
                {
                    wallet = new Wallet
                    {
                        Id        = Guid.NewGuid(),
                        UserId    = userId,
                        Balance   = 0,
                        CreatedAt = DateTime.UtcNow,
                    };
                    await _walletRepo.CreateAsync(wallet);
                }

                wallet.Balance   += refundAmount;
                wallet.UpdatedAt  = DateTime.UtcNow;
                await _walletRepo.UpdateAsync(wallet);

                await _walletTxRepo.CreateAsync(new WalletTransaction
                {
                    Id          = Guid.NewGuid(),
                    WalletId    = wallet.Id,
                    Amount      = refundAmount,
                    Type        = WalletTransactionType.Credit,
                    Reason      = WalletTransactionReason.Refund,
                    ReferenceId = bookingId,
                    Note        = $"Hoàn tiền hủy booking #{booking.Code} ({refundPercent}%)",
                    CreatedAt   = DateTime.UtcNow,
                });

                // Create Refund record for audit trail (auto-completed)
                if (payment != null)
                {
                    var existingRefund = await _refundRepository.GetByBookingIdAsync(bookingId);
                    if (existingRefund == null)
                    {
                        await _refundRepository.CreateAsync(new Refund
                        {
                            Id                = Guid.NewGuid(),
                            BookingId         = bookingId,
                            PaymentId         = payment.Id,
                            TotalRefundAmount = refundAmount,
                            InitiatedBy       = RefundInitinator.Customer,
                            Reason            = reason,
                            Status            = RefundStatus.Completed,
                            RefundedAt        = DateTime.UtcNow,
                            AdminNote         = $"Tự động hoàn {refundPercent}% vào ví",
                            CreatedAt         = DateTime.UtcNow,
                            UpdatedAt         = DateTime.UtcNow,
                        });
                    }
                }
            }

            // Cancel booking and its lines
            booking.Status    = BookingStatus.Cancelled;
            booking.UpdatedAt = DateTime.UtcNow;
            await _bookingRepository.UpdateAsync(booking);

            if (line != null)
            {
                line.Status    = BookingItineraryStatus.Cancelled;
                line.UpdatedAt = DateTime.UtcNow;
                await _bookingItineraryRepo.UpdateAsync(line);

                // Restore schedule spots
                var schedule = await _scheduleRepo.GetByIdAsync(line.ItineraryScheduleId);
                if (schedule != null)
                {
                    schedule.SpotLeft += line.NumberOfGuests;
                    await _scheduleRepo.UpdateAsync(schedule);
                }
            }

            // Notify customer
            string msg = refundAmount > 0
                ? $"Booking #{booking.Code} đã hủy. {refundPercent}% ({refundAmount:N0}đ) đã được hoàn vào ví của bạn."
                : $"Booking #{booking.Code} đã hủy. Không có hoàn tiền do hủy trong vòng 2 ngày trước ngày đi.";

            await _notificationRepo.CreateAsync(new Notification
            {
                Id              = Guid.NewGuid(),
                RecipientUserId = userId,
                Title           = "Đã hủy booking",
                Message         = msg,
                EntityType      = NotificationEntityType.Booking,
                EntityId        = bookingId,
                IsRead          = false,
                CreatedAt       = DateTime.UtcNow,
            });

            return new CancelBookingResultDTO
            {
                BookingId     = bookingId,
                BookingCode   = booking.Code,
                RefundPercent = refundPercent,
                RefundAmount  = refundAmount,
                Message       = msg,
            };
        }

        private async Task<Refund> CheckRefund(Guid refundId)
        {
            var refund = await _refundRepository.GetByIdAsync(refundId);
            if (refund == null)
                throw new KeyNotFoundException("Refund not found");
            return refund;
        }
    }
}
