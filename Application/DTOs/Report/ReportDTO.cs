using System;
using System.Collections.Generic;
using TouRest.Domain.Enums;

namespace TouRest.Application.DTOs.Report
{
    public class ReportDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? UserName { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public Guid ItemId { get; set; }
        public string? ItemName { get; set; }
        public ReportItemType ItemType { get; set; }
        public ReportStatus Status { get; set; }
        public List<string>? ImageUrls { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
