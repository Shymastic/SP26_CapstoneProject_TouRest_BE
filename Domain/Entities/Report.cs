using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TouRest.Domain.Base;
using TouRest.Domain.Enums;

namespace TouRest.Domain.Entities
{
    [Table("reports")]
    public class Report : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; } = null!;

        [Required]
        public Guid ItemId { get; set; }

        [Required]
        public ReportItemType ItemType { get; set; }

        [Required]
        public ReportStatus Status { get; set; }

        // Stored as a JSON array string e.g. ["url1","url2"]
        public string? ImageUrls { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
    }
}
