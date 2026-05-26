using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouRest.Domain.DTOs
{
    public class AgencyGuideDTO
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Avatar { get; set; }
        public int ActiveTours { get; set; }
        public int CompletedTotal { get; set; }
    }
}
