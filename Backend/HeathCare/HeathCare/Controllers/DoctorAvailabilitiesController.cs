using HeathCare.DTOs;
using HeathCare.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeathCare.Controllers
{
    [Route("api/doctors/{doctorId}/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DoctorAvailabilitiesController : ControllerBase
    {
        private readonly IDoctorAvailabilityService _availabilityService;
        private readonly ILogger<DoctorAvailabilitiesController> _logger;

        public DoctorAvailabilitiesController(
            IDoctorAvailabilityService availabilityService,
            ILogger<DoctorAvailabilitiesController> logger)
        {
            _availabilityService = availabilityService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorAvailabilityDTO>>> GetAvailabilities(int doctorId)
        {
            try
            {
                var availabilities = await _availabilityService.GetAvailabilitiesByDoctorAsync(doctorId);
                return Ok(availabilities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting availabilities for doctor {DoctorId}", doctorId);
                return StatusCode(500, "An error occurred while retrieving availabilities");
            }
        }

        [HttpGet("{id}", Name = "GetAvailability")]
        public async Task<ActionResult<DoctorAvailabilityDTO>> GetAvailability(int doctorId, int id)
        {
            try
            {
                var availability = await _availabilityService.GetAvailabilityByIdAsync(id);

                if (availability.DoctorId != doctorId)
                {
                    return BadRequest("Availability does not belong to the specified doctor");
                }

                return Ok(availability);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Availability not found: {AvailabilityId}", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting availability {AvailabilityId}", id);
                return StatusCode(500, "An error occurred while retrieving the availability");
            }
        }

        [HttpPost]
        public async Task<ActionResult<DoctorAvailabilityDTO>> CreateAvailability(int doctorId, DoctorAvailabilityCreateDTO availabilityDto)
        {
            try
            {
                if (doctorId != availabilityDto.DoctorId)
                {
                    return BadRequest("Doctor ID in path doesn't match request body");
                }

                var createdAvailability = await _availabilityService.CreateAvailabilityAsync(availabilityDto);

                return CreatedAtRoute("GetAvailability",
                    new { doctorId, id = createdAvailability.Id },
                    createdAvailability);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new { error = "Database error while saving availability" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating availability");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAvailability(
            int doctorId,
            int id,
            DoctorAvailabilityCreateDTO availabilityDto)
        {
            try
            {
                if (doctorId != availabilityDto.DoctorId)
                {
                    return BadRequest("Doctor ID in route doesn't match request body");
                }

                if (!await _availabilityService.AvailabilityExistsAsync(id))
                {
                    return NotFound();
                }

                var updatedAvailability = await _availabilityService.UpdateAvailabilityAsync(id, availabilityDto);
                return Ok(updatedAvailability);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Availability not found: {AvailabilityId}", id);
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid request to update availability");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating availability {AvailabilityId}", id);
                return StatusCode(500, "An error occurred while updating the availability");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAvailability(int doctorId, int id)
        {
            try
            {
                var availability = await _availabilityService.GetAvailabilityByIdAsync(id);

                if (availability.DoctorId != doctorId)
                {
                    return BadRequest("Availability does not belong to the specified doctor");
                }

                await _availabilityService.DeleteAvailabilityAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Availability not found: {AvailabilityId}", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting availability {AvailabilityId}", id);
                return StatusCode(500, "An error occurred while deleting the availability");
            }
        }
    }
}