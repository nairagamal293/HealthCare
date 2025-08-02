using AutoMapper;
using HealthCare.Data;
using HeathCare.DTOs;
using HeathCare.Models;
using HeathCare.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace HeathCare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DoctorsController> _logger;

        public DoctorsController(
            IDoctorService doctorService,
            IMapper mapper,
            IFileService fileService,
            ApplicationDbContext context,
            ILogger<DoctorsController> logger)
        {
            _doctorService = doctorService;
            _mapper = mapper;
            _fileService = fileService;
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorDTO>>> GetDoctors()
        {
            var doctors = await _doctorService.GetAllDoctorsAsync();
            return Ok(doctors);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorDTO>> GetDoctor(int id)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            if (doctor == null) return NotFound();
            return Ok(doctor);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DoctorDTO>> PostDoctor([FromForm] DoctorCreateDTO doctorDto)
        {
            try
            {
                var doctor = await _doctorService.CreateDoctorAsync(doctorDto);
                return CreatedAtAction(nameof(GetDoctor), new { id = doctor.Id }, doctor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating doctor");
                return StatusCode(500, "An error occurred while creating the doctor");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutDoctor(int id, [FromForm] DoctorUpdateDTO doctorDto)
        {
            if (id != doctorDto.Id)
                return BadRequest();

            try
            {
                // Get existing doctor with availabilities
                var doctor = await _context.Doctors
                    .Include(d => d.Availabilities)
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (doctor == null)
                    return NotFound();

                // Update basic doctor info
                _mapper.Map(doctorDto, doctor);

                // Handle image update if new file is provided
                if (doctorDto.ImageFile != null)
                {
                    // Delete old image
                    if (!string.IsNullOrEmpty(doctor.ImagePath))
                        _fileService.DeleteImage(doctor.ImagePath);

                    // Save new image
                    doctor.ImagePath = await _fileService.SaveImageAsync(doctorDto.ImageFile, "doctors");
                }

                // Parse availabilities from JSON if provided
                if (!string.IsNullOrEmpty(doctorDto.AvailabilitiesJson))
                {
                    var newAvailabilities = JsonSerializer.Deserialize<List<DoctorAvailabilityCreateDTO>>(doctorDto.AvailabilitiesJson);

                    // Clear existing availabilities
                    _context.DoctorAvailabilities.RemoveRange(doctor.Availabilities);

                    // Add new ones
                    foreach (var availDto in newAvailabilities)
                    {
                        var availability = new DoctorAvailability
                        {
                            DoctorId = doctor.Id,
                            DayOfWeek = (DayOfWeek)availDto.DayOfWeek,
                            StartTime = TimeSpan.Parse(availDto.StartTime),
                            EndTime = TimeSpan.Parse(availDto.EndTime),
                            IsAvailable = availDto.IsAvailable
                        };
                        doctor.Availabilities.Add(availability);
                    }
                }

                _context.Doctors.Update(doctor);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating doctor");
                return StatusCode(500, new { message = "An error occurred while updating the doctor" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            try
            {
                await _doctorService.DeleteDoctorAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}