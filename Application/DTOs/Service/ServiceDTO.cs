using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Enums;

namespace TouRest.Application.DTOs.Service
{
    public class ServiceDTO
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid ProviderId { get; set; }
        [Required]
        public string ProviderName { get; set; } = null!;
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = null!;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public int Price { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "DurationMinutes must be greater than 0")]
        public int DurationMinutes { get; set; }

        [Required]
        public ServiceStatus Status { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "BasePrice must be greater than 0")]
        public int BasePrice { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
