using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TouRest.Domain.Base;
using TouRest.Domain.Enums;

namespace TouRest.Domain.Entities
{
    [Table("providers")]
    public class Provider : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = null!;

        [Required]
        public ProviderStatus Status { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; } = null!;
        [Required]
        public decimal Latitude { get; set; }
        [Required]
        public decimal Longitude { get; set; }
        [Required]
        public string Address { get; set; } = null!;
        [Required]
        public TimeOnly StartTime { get; set; }
        [Required]
        public TimeOnly EndTime { get; set; }
        [Required]
        [MaxLength(255)]
        public string ContactEmail { get; set; } = null!;
        [Required]
        [MaxLength(20)]
        public string ContactPhone { get; set; } = null!;
        public Guid CreateByUserId { get; set; }
        public User User { get; set; } = null!;
        public ICollection<ProviderUser> ProviderUsers { get; set; } = [];

    }
}
