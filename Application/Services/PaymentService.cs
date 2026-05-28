using AutoMapper;
using PayOS;
using PayOS.Models.V2.PaymentRequests;
using PayOS.Models.Webhooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Payment;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IWalletTransactionRepository _walletTransactionRepository;
        private readonly PayOSClient _payOS;
        private readonly IMapper _mapper;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IBookingRepository bookingRepository,
            INotificationRepository notificationRepository,
            IWalletRepository walletRepository,
            IWalletTransactionRepository walletTransactionRepository,
            PayOSClient payOS,
            IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _bookingRepository = bookingRepository;
            _notificationRepository = notificationRepository;
            _walletRepository = walletRepository;
            _walletTransactionRepository = walletTransactionRepository;
            _payOS = payOS;
            _mapper = mapper;
        }

        public async Task<PaymentDTO> CreatePaymentAsync(Guid bookingId, Guid userId)
        {
            var booking = await ValidateBookingOwnership(bookingId, userId);
            if (booking.Status != BookingStatus.Pending)
                throw new InvalidOperationException("Booking is not in a payable state");


            var existingPayment = await _paymentRepository.GetActivePaymentByBookingIdAsync(bookingId);
            if (existingPayment != null)
            {
                await CancelPayOSLinkAsync(existingPayment);
                existingPayment.Status = PaymentStatus.Cancelled;
                await _paymentRepository.UpdateAsync(existingPayment);
            }


            // grossAmount = total before discount (bi.Price = baseAmount after BookingService fix)
            // discountAmount = bi.Price - bi.FinalPrice = discount applied at booking time
            // finalAmount = booking.TotalAmount (already NET = grossAmount - discount)
            var grossAmount = booking.BookingItineraries.Sum(bi => bi.Price);
            var discountAmount = booking.BookingItineraries.Sum(bi => bi.Price - bi.FinalPrice);
            var finalAmount = booking.TotalAmount;


            var orderCode = long.Parse(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()[^9..]);


            var paymentRequest = new CreatePaymentLinkRequest
            {
                OrderCode = orderCode,
                Amount = 2000, // test-only: fixed 2,000₫ sent to PayOS; real amount stored in DB
                Description = $"Booking {booking.Code}",
                CancelUrl = $"{Environment.GetEnvironmentVariable("APP_URL")}/payment/cancel",
                ReturnUrl = $"{Environment.GetEnvironmentVariable("APP_URL")}/payment/success",
                ExpiredAt = (int)DateTimeOffset.UtcNow.AddMinutes(15).ToUnixTimeSeconds()
            };

            var paymentLink = await _payOS.PaymentRequests.CreateAsync(paymentRequest);


            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                BookingId = bookingId,
                OrderCode = orderCode,
                Amount = grossAmount,
                DiscountAmount = discountAmount,
                FinalAmount = finalAmount,
                Status = PaymentStatus.Pending,
                PayOSPaymentLinkId = paymentLink.PaymentLinkId,
                CheckoutUrl = paymentLink.CheckoutUrl,
                ExpiredAt = DateTime.UtcNow.AddMinutes(15),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var created = await _paymentRepository.CreateAsync(payment);
            var dto = _mapper.Map<PaymentDTO>(created);
            dto.QrCode = paymentLink.QrCode;  // ← gán trực tiếp
            return dto;
        }

        public async Task HandleWebhookAsync(Webhook webhookData)
        {
            // Verify signature — throws if invalid
            var verifiedData = await _payOS.Webhooks.VerifyAsync(webhookData);

            var payment = await _paymentRepository.GetByOrderCodeAsync(verifiedData.OrderCode);
            if (payment == null) return;

            if (verifiedData.Code == "00") // success
            {
                payment.Status = PaymentStatus.Paid;
                payment.TransactionReference = verifiedData.Reference;
                payment.PaidAt = DateTime.UtcNow;
                payment.UpdatedAt = DateTime.UtcNow;
                await _paymentRepository.UpdateAsync(payment);

                // Confirm booking
                var booking = await _bookingRepository.GetByIdAsync(payment.BookingId);
                if (booking != null)
                {
                    booking.Status = BookingStatus.Confirmed;
                    booking.UpdatedAt = DateTime.UtcNow;
                    await _bookingRepository.UpdateAsync(booking);

                    await _notificationRepository.CreateAsync(new Notification
                    {
                        Id = Guid.NewGuid(),
                        RecipientUserId = booking.UserId,
                        Title = "Payment Confirmed",
                        Message = $"Your payment of {payment.FinalAmount:N0}đ for booking #{booking.Code} has been confirmed.",
                        EntityType = NotificationEntityType.Booking,
                        EntityId = payment.BookingId,
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow,
                    });
                    //after confirm booking, distribute earnings to agencies
                    var bookingWithDetails = await _bookingRepository.GetBookingWithItineraries(payment.BookingId);
                    if (bookingWithDetails != null)
                    {
                        // Group by agency
                        var agencyEarnings = bookingWithDetails.BookingItineraries
                            .GroupBy(bi => bi.ItinerarySchedule.Itinerary.AgencyId)
                            .Select(g => new
                            {
                                AgencyId = g.Key,
                                TotalEarning = g.Sum(bi => bi.FinalPrice)
                            });

                        foreach (var earning in agencyEarnings)
                        {
                            var agencyWallet = await _walletRepository.GetByAgencyIdAsync(earning.AgencyId);
                            if (agencyWallet != null)
                            {
                                agencyWallet.Balance += earning.TotalEarning;
                                agencyWallet.UpdatedAt = DateTime.UtcNow;
                                await _walletRepository.UpdateAsync(agencyWallet);

                                await _walletTransactionRepository.CreateAsync(new WalletTransaction
                                {
                                    Id = Guid.NewGuid(),
                                    WalletId = agencyWallet.Id,
                                    Amount = earning.TotalEarning,
                                    Type = WalletTransactionType.Credit,
                                    Reason = WalletTransactionReason.BookingEarning,
                                    ReferenceId = payment.BookingId,
                                    Note = $"Booking {booking.Code} payment received",
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow
                                });
                            }
                        }
                    }
                }
            }
            else // failed or cancelled
            {
                payment.Status = PaymentStatus.Failed;
                payment.UpdatedAt = DateTime.UtcNow;
                await _paymentRepository.UpdateAsync(payment);
            }
        }

        public async Task<PaymentDTO> CancelPaymentAsync(Guid bookingId, Guid userId)
        {
            await ValidateBookingOwnership(bookingId, userId);
            var payment = await _paymentRepository.GetActivePaymentByBookingIdAsync(bookingId);
            if (payment == null)
                throw new KeyNotFoundException("No active payment found");

            await CancelPayOSLinkAsync(payment);
            payment.Status = PaymentStatus.Cancelled;
            payment.CancelledReason = "Cancelled by user";
            payment.UpdatedAt = DateTime.UtcNow;

            var updated = await _paymentRepository.UpdateAsync(payment);
            return _mapper.Map<PaymentDTO>(updated);
        }

        public async Task<PaymentDTO> GetActivePaymentAsync(Guid bookingId)
        {
            var payment = await _paymentRepository.GetActivePaymentByBookingIdAsync(bookingId);
            if (payment == null)
                throw new KeyNotFoundException("No active payment found");
            return _mapper.Map<PaymentDTO>(payment);
        }

        private async Task CancelPayOSLinkAsync(Payment payment)
        {
            try
            {
                if (!string.IsNullOrEmpty(payment.PayOSPaymentLinkId))
                    await _payOS.PaymentRequests.CancelAsync(payment.PayOSPaymentLinkId);
            }
            catch (Exception)
            {

            }
        }
        private async Task<Booking> ValidateBookingOwnership(Guid bookingId, Guid userId)
        {
            var booking = await _bookingRepository.GetBookingWithItineraries(bookingId);
            if (booking == null)
                throw new KeyNotFoundException("Booking not found");
            if (booking.UserId != userId)
                throw new UnauthorizedAccessException("You do not own this booking");
            return booking;
        }
    }
}
