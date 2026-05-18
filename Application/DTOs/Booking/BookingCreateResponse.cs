namespace TouRest.Application.DTOs.Booking
{
    public class BookingCreateResponse
    {
        public Guid BookingId { get; set; }
        public string Code { get; set; } = "";
        public long TotalAmount { get; set; }
        public long BaseAmount { get; set; }
        public long DiscountAmount { get; set; }
        public string? VoucherApplied { get; set; }
    }
}
