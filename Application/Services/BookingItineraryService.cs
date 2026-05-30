using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.BookingItinerary;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class BookingItineraryService : IBookingItineraryService
    {
        private readonly IBookingItineraryRepository _bookingItineraryRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IItineraryScheduleRepository _itineraryScheduleRepository;
        private readonly IItineraryRepository _itineraryRepository;
        private readonly IMapper _mapper;
        public BookingItineraryService(IBookingItineraryRepository bookingItineraryRepository, IItineraryRepository itineraryRepository, 
            IBookingRepository bookingRepository, IItineraryScheduleRepository itineraryScheduleRepository, IMapper mapper)
        {
            _bookingItineraryRepository = bookingItineraryRepository;
            _itineraryRepository = itineraryRepository;
            _bookingRepository = bookingRepository;
            _itineraryScheduleRepository = itineraryScheduleRepository;
            _mapper = mapper;
        }
        public async Task<List<BookingItineraryDTO>> GetBookingItinerariesByBookingId(Guid bookingId)
        {
            var list = await _bookingItineraryRepository.GetBookingItinerariesByBookingId(bookingId);
            return _mapper.Map<List<BookingItineraryDTO>>(list);
        }
         public async Task<BookingItineraryDTO?> GetBookingItinerary(Guid id)
        {
            var bookingItinerary = await _bookingItineraryRepository.GetByIdAsync(id);
            return _mapper.Map<BookingItineraryDTO?>(bookingItinerary);
        }
            public async Task<BookingItineraryDTO> CreateBookingItinerary(Guid userId, BookingItineraryCreateRequest create)
        {
            
            var booking = await _bookingRepository.GetByIdAsync(create.BookingId);
            if (booking == null)
                throw new KeyNotFoundException("Booking not found");
            if (booking.UserId != userId)
                throw new UnauthorizedAccessException("You do not own this booking");
            if (booking.Status != BookingStatus.Pending)
                throw new InvalidOperationException("Booking is not in a modifiable state");

            
            var schedule = await _itineraryScheduleRepository.GetScheduleWithDetails(create.ItineraryScheduleId);
            if (schedule == null)
                throw new KeyNotFoundException("Schedule not found");

            var itinerary = schedule.Itinerary;
            if (schedule.SpotLeft < create.NumberOfGuests)
                throw new InvalidOperationException($"Only {schedule.SpotLeft} spots available");           
            var price = itinerary.Price * create.NumberOfGuests;
            var bookingItinerary = new BookingItinerary
            {
                Id = Guid.NewGuid(),
                BookingId = create.BookingId,
                ItineraryScheduleId = create.ItineraryScheduleId,
                Price = price,
                FinalPrice = price, 
                NumberOfGuests = create.NumberOfGuests,
                Status = BookingItineraryStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            schedule.SpotLeft -= create.NumberOfGuests;
            schedule.UpdatedAt = DateTime.UtcNow;
            await _itineraryScheduleRepository.UpdateAsync(schedule);

            booking.TotalAmount += price;
            booking.UpdatedAt = DateTime.UtcNow;
            await _bookingRepository.UpdateAsync(booking);

            var result = await _bookingItineraryRepository.CreateAsync(bookingItinerary);
            return _mapper.Map<BookingItineraryDTO>(result);
        }
        
         public async Task<BookingItineraryDTO> UpdateBookingItinerary(Guid id, Guid userId, BookingItineraryUpdateRequest update)
        {
            var bookingItinerary = await _bookingItineraryRepository.GetBookingItineraryWithDetails(id);
            if (bookingItinerary == null)
            {
                throw new KeyNotFoundException("Booking Itinerary not found");
            }
            if(bookingItinerary.Booking.UserId != userId)
            {
                throw new UnauthorizedAccessException("This user is not the one created this booking");
            }
            if (update.NumberOfGuests != null)
            {
                var difference = update.NumberOfGuests.Value - bookingItinerary.NumberOfGuests;
                var schedule = bookingItinerary.ItinerarySchedule;

                if (schedule.SpotLeft < difference)
                    throw new InvalidOperationException("Not enough spots available");

                schedule.SpotLeft -= difference;
                await _itineraryScheduleRepository.UpdateAsync(schedule);   
                bookingItinerary.NumberOfGuests = update.NumberOfGuests.Value;
            }
            if (update.VoucherId != null)
                bookingItinerary.VoucherId = update.VoucherId;
            if (update.Status != null)
                bookingItinerary.Status = update.Status.Value;
            var result = await _bookingItineraryRepository.UpdateAsync(bookingItinerary);
            return _mapper.Map<BookingItineraryDTO>(result);

        }
         public async Task DeleteBookingItinerary(Guid id, Guid userId)
        {
            var item = await _bookingItineraryRepository.GetByIdAsync(id);
            if (item == null)
                throw new KeyNotFoundException("BookingItinerary not found");
            if (item.Booking.UserId != userId)
                throw new UnauthorizedAccessException("You are not allow to delete this");
            item.Status = BookingItineraryStatus.Cancelled;
            await _bookingItineraryRepository.UpdateAsync(item);
        }
    }
}
