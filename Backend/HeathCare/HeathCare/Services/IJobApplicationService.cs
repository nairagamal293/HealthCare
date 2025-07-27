using HeathCare.DTOs;

namespace HeathCare.Services
{
    public interface IJobApplicationService
    {
        Task<IEnumerable<JobApplicationDTO>> GetAllApplicationsAsync();
        Task<IEnumerable<JobApplicationDTO>> GetApplicationsByCareerAsync(int careerId);
        Task<JobApplicationDTO> GetApplicationByIdAsync(int id);
        Task<JobApplicationDTO> CreateApplicationAsync(JobApplicationCreateDTO applicationDto);
        Task UpdateApplicationStatusAsync(int id, JobApplicationUpdateDTO applicationDto);
    }
}
