using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Enums;

namespace TouRest.Application.DTOs.Package
{
    public class PackageUpdateRequest
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "BasePrice must be greater than 0")]
        public int BasePrice { get; set; }

        [Required]
        public PackageStatus Status { get; set; }

        public List<Guid>? ServiceIds { get; set; }
    }
}
