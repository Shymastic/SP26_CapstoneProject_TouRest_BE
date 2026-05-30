namespace TouRest.Application.DTOs.Auth
{
    public class MeDTO
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string Role { get; set; } = null!;
        public string? SubRole { get; set; }
    }
}
