namespace TouRest.Application.DTOs.Refund
{
    public class CancelBookingResultDTO
    {
        public Guid BookingId { get; set; }
        public string BookingCode { get; set; } = null!;
        public int RefundPercent { get; set; }
        public long RefundAmount { get; set; }
        public string Message { get; set; } = null!;
    }
}
