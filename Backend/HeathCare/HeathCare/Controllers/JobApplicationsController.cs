using HeathCare.DTOs;
using HeathCare.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeathCare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobApplicationsController : ControllerBase
    {
        private readonly IJobApplicationService _jobApplicationService;
        private readonly ICareerService _careerService;

        public JobApplicationsController(
            IJobApplicationService jobApplicationService,
            ICareerService careerService)
        {
            _jobApplicationService = jobApplicationService;
            _careerService = careerService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<JobApplicationDTO>>> GetJobApplications()
        {
            var applications = await _jobApplicationService.GetAllApplicationsAsync();
            return Ok(applications);
        }

        [HttpGet("career/{careerId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<JobApplicationDTO>>> GetJobApplicationsByCareer(int careerId)
        {
            var applications = await _jobApplicationService.GetApplicationsByCareerAsync(careerId);
            return Ok(applications);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<JobApplicationDTO>> GetJobApplication(int id)
        {
            var application = await _jobApplicationService.GetApplicationByIdAsync(id);
            if (application == null) return NotFound();
            return Ok(application);
        }

        [HttpPost]
        public async Task<ActionResult<JobApplicationDTO>> PostJobApplication([FromForm] JobApplicationCreateDTO applicationDto)
        {
            // Verify the career is active
            var career = await _careerService.GetCareerByIdAsync(applicationDto.CareerId);
            if (career == null || !career.IsActive)
            {
                return BadRequest(new { message = "This career is not available for applications" });
            }

            var application = await _jobApplicationService.CreateApplicationAsync(applicationDto);
            return CreatedAtAction(nameof(GetJobApplication), new { id = application.Id }, application);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutJobApplication(int id, JobApplicationUpdateDTO applicationDto)
        {
            if (id != applicationDto.Id) return BadRequest();

            try
            {
                await _jobApplicationService.UpdateApplicationStatusAsync(id, applicationDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}