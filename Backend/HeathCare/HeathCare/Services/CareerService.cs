using AutoMapper;
using HealthCare.Data;
using HeathCare.DTOs;
using HeathCare.Models;
using Microsoft.EntityFrameworkCore;

namespace HeathCare.Services
{
    public class CareerService : ICareerService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CareerService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CareerDTO>> GetAllCareersAsync(bool activeOnly = true)
        {
            var query = _context.Careers.AsQueryable();

            if (activeOnly)
            {
                query = query.Where(c => c.IsActive &&
                    (c.ClosingDate == null || c.ClosingDate > DateTime.UtcNow));
            }

            var careers = await query
                .OrderByDescending(c => c.PostedDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CareerDTO>>(careers);
        }

        public async Task<CareerDTO> GetCareerByIdAsync(int id)
        {
            var career = await _context.Careers.FindAsync(id);
            if (career == null) return null;

            return _mapper.Map<CareerDTO>(career);
        }

        public async Task<CareerDTO> CreateCareerAsync(CareerCreateDTO careerDto)
        {
            var career = _mapper.Map<Career>(careerDto);
            career.PostedDate = DateTime.UtcNow;
            career.IsActive = true;

            _context.Careers.Add(career);
            await _context.SaveChangesAsync();

            return _mapper.Map<CareerDTO>(career);
        }

        public async Task UpdateCareerAsync(int id, CareerUpdateDTO careerDto)
        {
            if (id != careerDto.Id)
                throw new ArgumentException("ID mismatch");

            var career = await _context.Careers.FindAsync(id);
            if (career == null)
                throw new KeyNotFoundException("Career not found");

            _mapper.Map(careerDto, career);
            _context.Careers.Update(career);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCareerAsync(int id)
        {
            var career = await _context.Careers.FindAsync(id);
            if (career == null)
                throw new KeyNotFoundException("Career not found");

            _context.Careers.Remove(career);
            await _context.SaveChangesAsync();
        }
    }
}
