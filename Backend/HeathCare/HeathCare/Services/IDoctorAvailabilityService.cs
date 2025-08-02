using AutoMapper;
using HealthCare.Data;
using HeathCare.DTOs;
using HeathCare.Models;
using Microsoft.EntityFrameworkCore;

namespace HeathCare.Services
{
    public interface IDoctorAvailabilityService
    {
        Task<IEnumerable<DoctorAvailabilityDTO>> GetAvailabilitiesByDoctorAsync(int doctorId);
        Task<DoctorAvailabilityDTO> GetAvailabilityByIdAsync(int id);
        Task<DoctorAvailabilityDTO> CreateAvailabilityAsync(DoctorAvailabilityCreateDTO availabilityDto);
        Task<DoctorAvailabilityDTO> UpdateAvailabilityAsync(int id, DoctorAvailabilityCreateDTO availabilityDto);
        Task DeleteAvailabilityAsync(int id);
        Task<bool> AvailabilityExistsAsync(int id);
    }

    public class DoctorAvailabilityService : IDoctorAvailabilityService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<DoctorAvailabilityService> _logger;

        public DoctorAvailabilityService(
            ApplicationDbContext context,
            IMapper mapper,
            ILogger<DoctorAvailabilityService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<DoctorAvailabilityDTO>> GetAvailabilitiesByDoctorAsync(int doctorId)
        {
            try
            {
                var availabilities = await _context.DoctorAvailabilities
                    .Where(da => da.DoctorId == doctorId)
                    .OrderBy(da => da.DayOfWeek)
                    .ThenBy(da => da.StartTime)
                    .ToListAsync();

                return _mapper.Map<IEnumerable<DoctorAvailabilityDTO>>(availabilities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting availabilities for doctor {DoctorId}", doctorId);
                throw;
            }
        }

        public async Task<DoctorAvailabilityDTO> GetAvailabilityByIdAsync(int id)
        {
            try
            {
                var availability = await _context.DoctorAvailabilities
                    .FirstOrDefaultAsync(da => da.Id == id);

                if (availability == null)
                {
                    throw new KeyNotFoundException($"Availability with ID {id} not found");
                }

                return _mapper.Map<DoctorAvailabilityDTO>(availability);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting availability with ID {AvailabilityId}", id);
                throw;
            }
        }

        public async Task<DoctorAvailabilityDTO> CreateAvailabilityAsync(DoctorAvailabilityCreateDTO availabilityDto)
        {
            try
            {
                // Verify doctor exists
                var doctor = await _context.Doctors.FindAsync(availabilityDto.DoctorId);
                if (doctor == null)
                {
                    throw new ArgumentException($"Doctor with ID {availabilityDto.DoctorId} not found");
                }

                // Parse times
                if (!TimeSpan.TryParse(availabilityDto.StartTime, out var startTime) ||
                    !TimeSpan.TryParse(availabilityDto.EndTime, out var endTime))
                {
                    throw new ArgumentException("Times must be in HH:mm format");
                }

                // Validate time slot
                if (startTime >= endTime)
                {
                    throw new ArgumentException("End time must be after start time");
                }

                // Create and save
                var availability = new DoctorAvailability
                {
                    DoctorId = availabilityDto.DoctorId,
                    DayOfWeek = (DayOfWeek)availabilityDto.DayOfWeek, // Explicit cast
                    StartTime = startTime,
                    EndTime = endTime,
                    IsAvailable = availabilityDto.IsAvailable
                };

                _context.DoctorAvailabilities.Add(availability);
                await _context.SaveChangesAsync();

                return _mapper.Map<DoctorAvailabilityDTO>(availability);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error creating availability");
                throw new Exception("Database error while saving availability");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating availability");
                throw;
            }
        }

        public async Task<DoctorAvailabilityDTO> UpdateAvailabilityAsync(int id, DoctorAvailabilityCreateDTO availabilityDto)
        {
            try
            {
                var existingAvailability = await _context.DoctorAvailabilities.FindAsync(id);
                if (existingAvailability == null)
                {
                    throw new KeyNotFoundException($"Availability with ID {id} not found");
                }

                // Parse times
                if (!TimeSpan.TryParse(availabilityDto.StartTime, out var startTime) ||
                    !TimeSpan.TryParse(availabilityDto.EndTime, out var endTime))
                {
                    throw new ArgumentException("Invalid time format. Use HH:mm format");
                }

                if (startTime >= endTime)
                {
                    throw new ArgumentException("Start time must be before end time");
                }

                // Convert int to DayOfWeek for comparison
                var dayOfWeekEnum = (DayOfWeek)availabilityDto.DayOfWeek;

                // Check for overlapping availabilities (excluding current one)
                var hasOverlap = await _context.DoctorAvailabilities
                    .Where(da => da.DoctorId == availabilityDto.DoctorId)
                    .Where(da => da.DayOfWeek == dayOfWeekEnum) // Compare enum to enum
                    .Where(da => da.Id != id)
                    .AnyAsync(da => startTime < da.EndTime && endTime > da.StartTime);

                if (hasOverlap)
                {
                    throw new ArgumentException("This time slot overlaps with an existing availability");
                }

                // Update properties
                existingAvailability.DoctorId = availabilityDto.DoctorId;
                existingAvailability.DayOfWeek = dayOfWeekEnum; // Use converted enum
                existingAvailability.StartTime = startTime;
                existingAvailability.EndTime = endTime;
                existingAvailability.IsAvailable = availabilityDto.IsAvailable;

                _context.DoctorAvailabilities.Update(existingAvailability);
                await _context.SaveChangesAsync();

                return _mapper.Map<DoctorAvailabilityDTO>(existingAvailability);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating availability with ID {AvailabilityId}", id);
                throw;
            }
        }

        public async Task DeleteAvailabilityAsync(int id)
        {
            try
            {
                var availability = await _context.DoctorAvailabilities.FindAsync(id);
                if (availability == null)
                {
                    throw new KeyNotFoundException($"Availability with ID {id} not found");
                }

                _context.DoctorAvailabilities.Remove(availability);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting availability with ID {AvailabilityId}", id);
                throw;
            }
        }

        public async Task<bool> AvailabilityExistsAsync(int id)
        {
            return await _context.DoctorAvailabilities.AnyAsync(da => da.Id == id);
        }
    }

}