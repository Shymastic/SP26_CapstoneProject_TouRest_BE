using System.ComponentModel.DataAnnotations;

namespace TouRest.Application.DTOs.User
{
    public class CreateUserDTO
    {
        [Required, MaxLength(50)]
        public string Username { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, MinLength(6)]
        public string Password { get; set; } = null!;

        [Phone]
        public string? Phone { get; set; }

        [Required]
        public string RoleCode { get; set; } = null!;

        public Guid? ProviderId { get; set; }

        public Guid? AgencyId { get; set; }
    }
}
