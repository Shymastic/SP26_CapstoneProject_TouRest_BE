namespace TouRest.Application.DTOs.Booking
{
    public class BookingPassengerDTO
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string IdNumber { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public int Age { get; set; }
    }

    public class PassengerRequest
    {
        public string FullName { get; set; } = null!;
        public string IdNumber { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public int Age { get; set; }
    }
}
