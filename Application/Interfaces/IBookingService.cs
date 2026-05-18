using TouRest.Application.DTOs.Booking;

namespace TouRest.Application.Interfaces
{
    public interface IBookingService
    {
        Task<BookingDTO> GetBookingAsync(Guid id, Guid userId, bool isAdmin);
        Task<BookingCreateResponse> CreateBookingAsync(BookingCreateRequest request, Guid userId);
        Task<BookingDTO> UpdateBookingAsync(Guid id, Guid userId, bool isAdmin, BookingUpdateRequest request);
        Task DeleteBookingAsync(Guid id, Guid userId, bool isAdmin);
        Task<List<BookingDTO>> GetBookingsByUserIdAsync(Guid userId);
    }
}
