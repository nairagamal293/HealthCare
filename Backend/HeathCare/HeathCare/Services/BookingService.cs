using AutoMapper;
using HealthCare.Data;
using HeathCare.DTOs;
using HeathCare.Models;
using Microsoft.EntityFrameworkCore;

namespace HeathCare.Services
{
    // Services/BookingService.cs
    public interface IBookingService
    {
        Task<IEnumerable<BookingDTO>> GetAllBookingsAsync();
        Task<BookingDTO> GetBookingByIdAsync(int id);
        Task<BookingDTO> CreateBookingAsync(BookingCreateDTO bookingDto);
        Task UpdateBookingStatusAsync(int id, BookingUpdateDTO bookingDto);
        Task DeleteBookingAsync(int id);
    }

    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BookingService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BookingDTO>> GetAllBookingsAsync()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Department)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<BookingDTO>>(bookings);
        }

        public async Task<BookingDTO> GetBookingByIdAsync(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Department)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return null;

            return _mapper.Map<BookingDTO>(booking);
        }

        public async Task<BookingDTO> CreateBookingAsync(BookingCreateDTO bookingDto)
        {
            var booking = _mapper.Map<Booking>(bookingDto);
            booking.CreatedAt = DateTime.UtcNow;

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // Load department name for the response
            await _context.Entry(booking)
                .Reference(b => b.Department)
                .LoadAsync();

            return _mapper.Map<BookingDTO>(booking);
        }

        public async Task UpdateBookingStatusAsync(int id, BookingUpdateDTO bookingDto)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) throw new KeyNotFoundException("Booking not found");

            booking.IsConfirmed = bookingDto.IsConfirmed;
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBookingAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) throw new KeyNotFoundException("Booking not found");

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
        }
    }
}
