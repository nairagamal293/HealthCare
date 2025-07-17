using AutoMapper;
using HealthCare.Data;
using HeathCare.DTOs;
using HeathCare.Models;
using Microsoft.EntityFrameworkCore;

namespace HeathCare.Services
{
    // Services/DoctorService.cs
    public class DoctorService : IDoctorService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<DoctorService> _logger;

        public DoctorService(
            ApplicationDbContext context,
            IMapper mapper,
            IFileService fileService,
            IWebHostEnvironment environment,
            ILogger<DoctorService> logger)
        {
            _context = context;
            _mapper = mapper;
            _fileService = fileService;
            _environment = environment;
            _logger = logger;
        }

        public async Task<DoctorDTO> CreateDoctorAsync(DoctorCreateDTO doctorDto)
        {
            try
            {
                // Validate department exists
                var department = await _context.Departments.FindAsync(doctorDto.DepartmentId);
                if (department == null)
                {
                    throw new ArgumentException("Department not found");
                }

                // Handle file upload
                string imagePath = null;
                if (doctorDto.ImageFile != null && doctorDto.ImageFile.Length > 0)
                {
                    try
                    {
                        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "doctors");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var uniqueFileName = $"{Guid.NewGuid()}_{doctorDto.ImageFile.FileName}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await doctorDto.ImageFile.CopyToAsync(stream);
                        }

                        imagePath = $"/uploads/doctors/{uniqueFileName}";
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error saving doctor image");
                        throw new Exception("Could not save image file");
                    }
                }

                var doctor = new Doctor
                {
                    Name = doctorDto.Name,
                    Specialty = doctorDto.Specialty,
                    Bio = doctorDto.Bio,
                    ImagePath = imagePath,
                    DepartmentId = doctorDto.DepartmentId
                };

                _context.Doctors.Add(doctor);
                await _context.SaveChangesAsync();

                // Load department name for the DTO
                await _context.Entry(doctor)
                    .Reference(d => d.Department)
                    .LoadAsync();

                return _mapper.Map<DoctorDTO>(doctor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating doctor");
                throw;
            }
        }


        public async Task UpdateDoctorAsync(int id, DoctorUpdateDTO doctorDto)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) throw new KeyNotFoundException("Doctor not found");

            // Handle image update if new file is provided
            if (doctorDto.ImageFile != null)
            {
                // Delete old image
                if (!string.IsNullOrEmpty(doctor.ImagePath))
                    _fileService.DeleteImage(doctor.ImagePath);

                // Save new image
                doctor.ImagePath = await _fileService.SaveImageAsync(doctorDto.ImageFile);
            }

            _mapper.Map(doctorDto, doctor);
            _context.Doctors.Update(doctor);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDoctorAsync(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) throw new KeyNotFoundException("Doctor not found");

            // Delete associated image
            if (!string.IsNullOrEmpty(doctor.ImagePath))
                _fileService.DeleteImage(doctor.ImagePath);

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();
        }

        // In DoctorService.cs, update GetDoctorByIdAsync and GetAllDoctorsAsync:

        // Services/DoctorService.cs
        // Services/DoctorService.cs
        public async Task<DoctorDTO> GetDoctorByIdAsync(int id)
        {
            var doctor = await _context.Doctors
                .Include(d => d.Department)
                .Include(d => d.Reviews) // Include reviews
                .FirstOrDefaultAsync(d => d.Id == id);

            if (doctor == null) return null;

            var doctorDTO = _mapper.Map<DoctorDTO>(doctor);

            // These will be automatically mapped from the calculated properties
            return doctorDTO;
        }

        public async Task<IEnumerable<DoctorDTO>> GetAllDoctorsAsync()
        {
            var doctors = await _context.Doctors
                .Include(d => d.Department)
                .Include(d => d.Reviews)
                .ToListAsync();

            return _mapper.Map<IEnumerable<DoctorDTO>>(doctors);
        }

    }

    public interface IDoctorService
    {
        Task<IEnumerable<DoctorDTO>> GetAllDoctorsAsync();
        Task<DoctorDTO> GetDoctorByIdAsync(int id);
        Task<DoctorDTO> CreateDoctorAsync(DoctorCreateDTO doctorDto);
        Task UpdateDoctorAsync(int id, DoctorUpdateDTO doctorDto);
        Task DeleteDoctorAsync(int id);
    }
}
