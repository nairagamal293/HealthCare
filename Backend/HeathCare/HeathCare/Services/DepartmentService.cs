using AutoMapper;
using HealthCare.Data;
using HeathCare.DTOs;
using HeathCare.DTOs.HeathCare.DTOs;
using HeathCare.Models;
using HeathCare.Models.HeathCare.Models;
using Microsoft.EntityFrameworkCore;
using static HeathCare.Services.DepartmentService;

namespace HeathCare.Services
{
    // Services/DepartmentService.cs
    public class DepartmentService : IDepartmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        private readonly ILogger<DepartmentService> _logger;

        public DepartmentService(
            ApplicationDbContext context,
            IMapper mapper,
            IFileService fileService,
            ILogger<DepartmentService> logger)
        {
            _context = context;
            _mapper = mapper;
            _fileService = fileService;
            _logger = logger;
        }

        // In DepartmentService.cs, update GetAllDepartmentsAsync and GetDepartmentByIdAsync:

        public async Task<IEnumerable<DepartmentDTO>> GetAllDepartmentsAsync()
        {
            var departments = await _context.Departments
                .Include(d => d.Doctors)
                .Include(d => d.Services) // Add this line
                .ToListAsync();

            return _mapper.Map<IEnumerable<DepartmentDTO>>(departments);
        }

        public async Task<DepartmentDTO> GetDepartmentByIdAsync(int id)
        {
            var department = await _context.Departments
                .Include(d => d.Doctors)
                .Include(d => d.Services) // Add this line
                .FirstOrDefaultAsync(d => d.Id == id);

            if (department == null) return null;

            var departmentDTO = _mapper.Map<DepartmentDTO>(department);
            departmentDTO.DoctorCount = department.Doctors?.Count ?? 0;
            return departmentDTO;
        }

        public async Task<DepartmentDTO> CreateDepartmentAsync(DepartmentCreateDTO departmentDto)
        {
            // Handle file upload
            string imagePath = null;
            if (departmentDto.ImageFile != null && departmentDto.ImageFile.Length > 0)
            {
                try
                {
                    imagePath = await _fileService.SaveImageAsync(departmentDto.ImageFile, "departments");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving department image");
                    throw new Exception("Could not save image file");
                }
            }

            var department = new Department
            {
                Name = departmentDto.Name,
                Description = departmentDto.Description,
                ImagePath = imagePath
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            // Add services if they exist
            if (departmentDto.Services != null && departmentDto.Services.Any())
            {
                foreach (var serviceDto in departmentDto.Services)
                {
                    var service = new Service
                    {
                        Name = serviceDto.Name,
                        Description = serviceDto.Description,
                        DepartmentId = department.Id
                    };
                    _context.Services.Add(service);
                }
                await _context.SaveChangesAsync();
            }

            return _mapper.Map<DepartmentDTO>(department);
        }

        public async Task UpdateDepartmentAsync(int id, DepartmentUpdateDTO departmentDto)
        {
            var department = await _context.Departments
                .Include(d => d.Services)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (department == null) throw new KeyNotFoundException("Department not found");

            // Handle image update if new file is provided
            if (departmentDto.ImageFile != null)
            {
                // Delete old image
                if (!string.IsNullOrEmpty(department.ImagePath))
                    _fileService.DeleteImage(department.ImagePath);

                // Save new image
                department.ImagePath = await _fileService.SaveImageAsync(departmentDto.ImageFile, "departments");
            }

            // Update department properties
            department.Name = departmentDto.Name;
            department.Description = departmentDto.Description;

            // Handle services - only update if new services are provided
            if (departmentDto.Services != null && departmentDto.Services.Any())
            {
                // Remove existing services
                _context.Services.RemoveRange(department.Services);

                // Add new services
                foreach (var serviceDto in departmentDto.Services)
                {
                    department.Services.Add(new Service
                    {
                        Name = serviceDto.Name,
                        Description = serviceDto.Description,
                        DepartmentId = department.Id
                    });
                }
            }

            _context.Departments.Update(department);
            await _context.SaveChangesAsync();
        }



        public async Task DeleteDepartmentAsync(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null) throw new KeyNotFoundException("Department not found");

            // Delete associated image
            if (!string.IsNullOrEmpty(department.ImagePath))
                _fileService.DeleteImage(department.ImagePath);

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
        }

        public interface IDepartmentService
        {
            Task<IEnumerable<DepartmentDTO>> GetAllDepartmentsAsync();
            Task<DepartmentDTO> GetDepartmentByIdAsync(int id);
            Task<DepartmentDTO> CreateDepartmentAsync(DepartmentCreateDTO departmentDto);
            Task UpdateDepartmentAsync(int id, DepartmentUpdateDTO departmentDto);
            Task DeleteDepartmentAsync(int id);
        }
    }
}