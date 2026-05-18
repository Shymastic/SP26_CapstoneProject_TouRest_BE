namespace TouRest.Application.DTOs.User
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? FullName { get; set; }
        public string Role { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? AddressDetail { get; set; }
        public string? CityId { get; set; }
        public string? DistrictId { get; set; }
        public string? ImageUrl { get; set; }
    }
}
