using System.ComponentModel.DataAnnotations;

namespace TouRest.Application.DTOs.Agency
{
    public class CreateGuideRequest
    {
        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; } = null!;

        [Required, MaxLength(255)]
        public string FullName { get; set; } = null!;

        [Required, MinLength(6)]
        public string Password { get; set; } = null!;

        [MaxLength(20)]
        public string? Phone { get; set; }
    }
}
