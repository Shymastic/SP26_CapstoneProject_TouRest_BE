using System.ComponentModel.DataAnnotations;

namespace TouRest.Application.DTOs.User
{
    public class AdminUpdateUserDTO
    {
        [Required, MaxLength(100)]
        public string Username { get; set; } = null!;

        [MaxLength(255)]
        public string? FullName { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [Required]
        public string Status { get; set; } = null!;   // Active | Inactive | Locked

        [Required]
        public string RoleCode { get; set; } = null!;  // CUSTOMER | ADMIN | PROVIDER | AGENCY

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(500)]
        public string? AddressDetail { get; set; }

        [MaxLength(100)]
        public string? CityId { get; set; }

        [MaxLength(100)]
        public string? DistrictId { get; set; }
    }
}
