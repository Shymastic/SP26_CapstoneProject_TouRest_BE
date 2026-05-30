using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Enums;

namespace TouRest.Application.DTOs.BookingItinerary
{
    public class BookingItineraryDTO
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }
        public Guid ItineraryScheduleId { get; set; }
        public Guid? VoucherId { get; set; }
        public long Price { get; set; }
        public long FinalPrice { get; set; }
        public int NumberOfGuests { get; set; }
        public BookingItineraryStatus Status { get; set; }
        public string? ItineraryName { get; set; }     
        public DateTime? ScheduleStartTime { get; set; } 
        public DateTime? ScheduleEndTime { get; set; }   
        public string? VoucherCode { get; set; }          
        public long DiscountAmount { get; set; }         
        public bool HasFeedback { get; set; }

        // Journey detail fields
        public Guid? ItineraryId { get; set; }
        public string? GuideName { get; set; }
        public string? GuidePhone { get; set; }
    }
}
