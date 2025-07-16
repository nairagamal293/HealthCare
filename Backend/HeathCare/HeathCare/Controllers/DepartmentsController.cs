using HeathCare.DTOs;
using HeathCare.DTOs.HeathCare.DTOs;
using HeathCare.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using static HeathCare.Services.DepartmentService;

namespace HeathCare.Controllers
{
    // Controllers/DepartmentsController.cs
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        private readonly IServiceService _serviceService; // Add this line
        private readonly ILogger<DoctorService> _logger;

        public DepartmentsController(IDepartmentService departmentService, IServiceService serviceService, ILogger<DoctorService> logger)
        {
            _departmentService = departmentService;
            _serviceService = serviceService; // Add this line
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentDTO>>> GetDepartments()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            return Ok(departments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentDTO>> GetDepartment(int id)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            if (department == null) return NotFound();
            return Ok(department);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DepartmentDTO>> PostDepartment([FromForm] DepartmentCreateWithServicesDTO departmentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var departmentCreateDto = new DepartmentCreateDTO
                {
                    Name = departmentDto.Name,
                    Description = departmentDto.Description,
                    ImageFile = departmentDto.ImageFile,
                    Services = JsonSerializer.Deserialize<List<ServiceCreateDTO>>(departmentDto.ServicesJson)
                };

                var department = await _departmentService.CreateDepartmentAsync(departmentCreateDto);
                return CreatedAtAction(nameof(GetDepartment), new { id = department.Id }, department);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating department");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutDepartment(int id, [FromForm] DepartmentUpdateWithServicesDTO departmentDto)
        {
            if (id != departmentDto.Id)
                return BadRequest();

            try
            {
                var departmentUpdateDto = new DepartmentUpdateDTO
                {
                    Id = departmentDto.Id,
                    Name = departmentDto.Name,
                    Description = departmentDto.Description,
                    ImageFile = departmentDto.ImageFile,
                    Services = JsonSerializer.Deserialize<List<ServiceCreateDTO>>(departmentDto.ServicesJson)
                };

                await _departmentService.UpdateDepartmentAsync(id, departmentUpdateDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating department");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            try
            {
                await _departmentService.DeleteDepartmentAsync(id);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}