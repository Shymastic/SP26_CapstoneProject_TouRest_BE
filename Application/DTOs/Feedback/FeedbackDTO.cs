using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Enums;

namespace TouRest.Application.DTOs.Feedback
{
    public class FeedbackDTO
    {
        public Guid BookingItineraryId { get; set; }
        public FeedbackItemType ItemType { get; set; }
        public Guid ItemId { get; set; }
        public Guid ItineraryId { get; set; }
        public string? ItineraryName { get; set; }
        public int Rating { get; set; }
        public string Title { get; set; } = null!;
        public string? Comment { get; set; }
        public string? AgencyReply { get; set; }
        public DateTime? RepliedAt { get; set; }
        public string? Username { get; set; }
        public string? UserAvatar { get; set; }
        public bool IsAnonymous { get; set; }
        public FeedbackStatus Status { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
