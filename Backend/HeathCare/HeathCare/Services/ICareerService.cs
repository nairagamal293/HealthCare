using HeathCare.DTOs;

namespace HeathCare.Services
{
    public interface ICareerService
    {
        Task<IEnumerable<CareerDTO>> GetAllCareersAsync(bool activeOnly = true);
        Task<CareerDTO> GetCareerByIdAsync(int id);
        Task<CareerDTO> CreateCareerAsync(CareerCreateDTO careerDto);
        Task UpdateCareerAsync(int id, CareerUpdateDTO careerDto);
        Task DeleteCareerAsync(int id);
    }
}
