using HeathCare.DTOs;
using HeathCare.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeathCare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CareersController : ControllerBase
    {
        private readonly ICareerService _careerService;

        public CareersController(ICareerService careerService)
        {
            _careerService = careerService;
        }

        // Public endpoints - no authorization required
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CareerDTO>>> GetCareers([FromQuery] bool activeOnly = true)
        {
            var careers = await _careerService.GetAllCareersAsync(activeOnly);
            return Ok(careers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CareerDTO>> GetCareer(int id)
        {
            var career = await _careerService.GetCareerByIdAsync(id);
            if (career == null) return NotFound();
            return Ok(career);
        }

        // Protected endpoints - require Admin role
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CareerDTO>> PostCareer(CareerCreateDTO careerDto)
        {
            var career = await _careerService.CreateCareerAsync(careerDto);
            return CreatedAtAction(nameof(GetCareer), new { id = career.Id }, career);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutCareer(int id, CareerUpdateDTO careerDto)
        {
            if (id != careerDto.Id) return BadRequest();

            try
            {
                await _careerService.UpdateCareerAsync(id, careerDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCareer(int id)
        {
            try
            {
                await _careerService.DeleteCareerAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}