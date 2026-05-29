using PayOS.Models.Webhooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Payment;

namespace TouRest.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentDTO> CreatePaymentAsync(Guid bookingId, Guid userId);
        Task<PaymentDTO> CancelPaymentAsync(Guid bookingId, Guid userId);
        Task HandleWebhookAsync(Webhook webhookData);
        Task<PaymentDTO> GetActivePaymentAsync(Guid bookingId);
        Task<PaymentDTO?> GetLatestPaymentAsync(Guid bookingId);
        Task FinalizePaymentByOrderCodeAsync(long orderCode);
    }
}
