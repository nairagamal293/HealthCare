// Services/JobApplicationService.cs
using AutoMapper;
using HealthCare.Data;
using HeathCare.DTOs;
using HeathCare.Models;
using Microsoft.EntityFrameworkCore;

namespace HeathCare.Services
{
    public class JobApplicationService : IJobApplicationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        private readonly ILogger<JobApplicationService> _logger;

        public JobApplicationService(
            ApplicationDbContext context,
            IMapper mapper,
            IFileService fileService,
            ILogger<JobApplicationService> logger)
        {
            _context = context;
            _mapper = mapper;
            _fileService = fileService;
            _logger = logger;
        }

        public async Task<IEnumerable<JobApplicationDTO>> GetAllApplicationsAsync()
        {
            var applications = await _context.JobApplications
                .Include(a => a.Career)
                .OrderByDescending(a => a.ApplicationDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<JobApplicationDTO>>(applications);
        }

        public async Task<IEnumerable<JobApplicationDTO>> GetApplicationsByCareerAsync(int careerId)
        {
            var applications = await _context.JobApplications
                .Include(a => a.Career)
                .Where(a => a.CareerId == careerId)
                .OrderByDescending(a => a.ApplicationDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<JobApplicationDTO>>(applications);
        }

        public async Task<JobApplicationDTO> GetApplicationByIdAsync(int id)
        {
            var application = await _context.JobApplications
                .Include(a => a.Career)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (application == null) return null;

            return _mapper.Map<JobApplicationDTO>(application);
        }

        public async Task<JobApplicationDTO> CreateApplicationAsync(JobApplicationCreateDTO applicationDto)
        {
            // Verify career exists
            var career = await _context.Careers.FindAsync(applicationDto.CareerId);
            if (career == null)
            {
                throw new ArgumentException("Career not found");
            }

            // Handle CV file upload
            string cvPath = null;
            if (applicationDto.CvFile != null && applicationDto.CvFile.Length > 0)
            {
                try
                {
                    // Check if file is PDF
                    if (Path.GetExtension(applicationDto.CvFile.FileName).ToLower() != ".pdf")
                    {
                        throw new ArgumentException("Only PDF files are allowed for CV");
                    }

                    cvPath = await _fileService.SaveFileAsync(applicationDto.CvFile, "cvs");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving CV file");
                    throw new Exception("Could not save CV file");
                }
            }

            var application = _mapper.Map<JobApplication>(applicationDto);
            application.CvPath = cvPath;
            application.ApplicationDate = DateTime.UtcNow;

            _context.JobApplications.Add(application);
            await _context.SaveChangesAsync();

            // Load career title for the DTO
            await _context.Entry(application)
                .Reference(a => a.Career)
                .LoadAsync();

            return _mapper.Map<JobApplicationDTO>(application);
        }

        public async Task UpdateApplicationStatusAsync(int id, JobApplicationUpdateDTO applicationDto)
        {
            if (id != applicationDto.Id)
                throw new ArgumentException("ID mismatch");

            var application = await _context.JobApplications.FindAsync(id);
            if (application == null)
                throw new KeyNotFoundException("Application not found");

            application.IsReviewed = applicationDto.IsReviewed;
            _context.JobApplications.Update(application);
            await _context.SaveChangesAsync();
        }
    }
}