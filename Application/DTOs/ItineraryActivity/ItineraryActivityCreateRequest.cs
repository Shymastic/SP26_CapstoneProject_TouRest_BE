using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouRest.Application.DTOs.ItineraryActivity
{
    public class ItineraryActivityCreateRequest
    {
        [Required]
        public Guid ItineraryStopId { get; set; }
        public Guid? ServiceId { get; set; }

        [MaxLength(255)]
        public string? CustomName { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "ActivityOrder must be greater than or equal to 0")]
        public int ActivityOrder { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Range(0, int.MaxValue)]
        public int Price { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }
    }
}
