using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayOS.Models.Webhooks;
using System.Security.Claims;
using TouRest.Api.Common;
using TouRest.Api.Extensions;
using TouRest.Application.DTOs.Payment;
using TouRest.Application.Interfaces;
using TouRest.Application.Services;

namespace TouRest.Api.Controllers
{
    [ApiController]
    [Route("api/payment")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Create a payment for a booking
        /// </summary>
        [HttpPost("create/{bookingId}")]
        public async Task<IActionResult> CreatePayment(Guid bookingId)
        {
            var userId = User.GetUserId();
            var result = await _paymentService.CreatePaymentAsync(bookingId, userId);
            return ApiResponseFactory.Created(result, "Payment link created");
        }
        
        /// <summary>
        /// Cancel an active payment for a booking
        /// </summary>
        [HttpPost("cancel/{bookingId}")]
        public async Task<IActionResult> CancelPayment(Guid bookingId)
        {
            var userId = User.GetUserId();
            var result = await _paymentService.CancelPaymentAsync(bookingId, userId);
            return ApiResponseFactory.Ok(result, "Payment link canceled");
        }

        /// <summary>
        /// Get active payment for a booking (if exists)
        /// </summary>
        [HttpGet("active/{bookingId}")]
        public async Task<IActionResult> GetActivePayment(Guid bookingId)
        {
            var result = await _paymentService.GetActivePaymentAsync(bookingId);
            if (result == null)
                return NotFound(new { message = "No active payment found for this booking." });
            return ApiResponseFactory.Ok(result);
        }

        /// <summary>
        /// Get latest payment for a booking (any status) — used by frontend polling to detect Paid
        /// </summary>
        [HttpGet("latest/{bookingId}")]
        public async Task<IActionResult> GetLatestPayment(Guid bookingId)
        {
            var result = await _paymentService.GetLatestPaymentAsync(bookingId);
            if (result == null)
                return NotFound(new { message = "No payment found for this booking." });
            return ApiResponseFactory.Ok(result);
        }

        /// <summary>
        /// Finalize payment after PayOS redirect — verifies with PayOS API and confirms DB state.
        /// Call this from the success page when code=00 is in the return URL.
        /// </summary>
        [HttpPost("finalize/{orderCode:long}")]
        public async Task<IActionResult> FinalizePayment(long orderCode)
        {
            await _paymentService.FinalizePaymentByOrderCodeAsync(orderCode);
            return ApiResponseFactory.Ok("Payment finalized");
        }

        /// <summary>
        /// PayOS webhook — called by PayOS after payment completes. Must be AllowAnonymous.
        /// </summary>
        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> Webhook([FromBody] Webhook webhookData)
        {
            try
            {
                await _paymentService.HandleWebhookAsync(webhookData);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Webhook handling failed");
                return Ok(new { success = false }); // Always return 200 so PayOS doesn't retry
            }
        }
    }
}