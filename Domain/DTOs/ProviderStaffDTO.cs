namespace TouRest.Domain.DTOs
{
    public class ProviderTourGroupDTO
    {
        public Guid ScheduleId { get; set; }
        public string AgencyName { get; set; } = null!;
        public string TourName { get; set; } = null!;
        public string? TourDescription { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; } = null!;
        public int TotalPatients { get; set; }
        public int SentCount { get; set; }
    }

    public class ProviderPatientDTO
    {
        public Guid BookingItineraryId { get; set; }
        public Guid BookingId { get; set; }
        public string BookingCode { get; set; } = null!;
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int NumberOfGuests { get; set; }
        public bool ResultSent { get; set; }
        public DateTime? SentAt { get; set; }
    }

    public class ProviderPassengerDTO
    {
        public Guid PassengerId { get; set; }
        public Guid BookingId { get; set; }
        public string BookingCode { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string IdNumber { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public int Age { get; set; }
        public bool ResultSent { get; set; }
        public DateTime? SentAt { get; set; }
    }

    public class PassengerMedicalResultDTO
    {
        public Guid PassengerId { get; set; }
        public string FullName { get; set; } = null!;
        public int Age { get; set; }
        public string IdNumber { get; set; } = null!;
        public bool ResultSent { get; set; }
        public DateTime? SentAt { get; set; }
        public string? Notes { get; set; }
        public List<string> ImageUrls { get; set; } = [];
    }

    public class BookingStopMedicalResultDTO
    {
        public string ProviderName { get; set; } = null!;
        public string StopName { get; set; } = null!;
        public List<PassengerMedicalResultDTO> Passengers { get; set; } = [];
    }
}
