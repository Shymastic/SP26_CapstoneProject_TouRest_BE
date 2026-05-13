using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TouRest.Domain.Enums;

namespace TouRest.Application.DTOs.Report
{
    public class ReportCreateRequest
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

        public List<string>? ImageUrls { get; set; }
    }
}
