// Services/ServiceService.cs
using AutoMapper;
using HealthCare.Data;
using HeathCare.DTOs;
using HeathCare.Models;
using Microsoft.EntityFrameworkCore;

namespace HeathCare.Services
{
    public interface IServiceService
    {
        Task<IEnumerable<ServiceDTO>> GetAllServicesAsync();
        Task<IEnumerable<ServiceDTO>> GetServicesByDepartmentAsync(int departmentId);
        Task<ServiceDTO> GetServiceByIdAsync(int id);
        Task<ServiceDTO> CreateServiceAsync(ServiceCreateDTO serviceDto);
        Task UpdateServiceAsync(int id, ServiceUpdateDTO serviceDto);
        Task DeleteServiceAsync(int id);
    }

    public class ServiceService : IServiceService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ServiceService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ServiceDTO>> GetAllServicesAsync()
        {
            var services = await _context.Services
                .Include(s => s.Department)
                .ToListAsync();
            return _mapper.Map<IEnumerable<ServiceDTO>>(services);
        }

        public async Task<IEnumerable<ServiceDTO>> GetServicesByDepartmentAsync(int departmentId)
        {
            var services = await _context.Services
                .Include(s => s.Department)
                .Where(s => s.DepartmentId == departmentId)
                .ToListAsync();
            return _mapper.Map<IEnumerable<ServiceDTO>>(services);
        }

        public async Task<ServiceDTO> GetServiceByIdAsync(int id)
        {
            var service = await _context.Services
                .Include(s => s.Department)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (service == null) return null;

            return _mapper.Map<ServiceDTO>(service);
        }

        public async Task<ServiceDTO> CreateServiceAsync(ServiceCreateDTO serviceDto)
        {
            var department = await _context.Departments.FindAsync(serviceDto.DepartmentId);
            if (department == null)
            {
                throw new ArgumentException("Department not found");
            }

            var service = _mapper.Map<Service>(serviceDto);
            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            // Load department name for the DTO
            await _context.Entry(service)
                .Reference(s => s.Department)
                .LoadAsync();

            return _mapper.Map<ServiceDTO>(service);
        }

        public async Task UpdateServiceAsync(int id, ServiceUpdateDTO serviceDto)
        {
            if (id != serviceDto.Id)
                throw new ArgumentException("ID mismatch");

            var service = await _context.Services.FindAsync(id);
            if (service == null)
                throw new KeyNotFoundException("Service not found");

            _mapper.Map(serviceDto, service);
            _context.Services.Update(service);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteServiceAsync(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
                throw new KeyNotFoundException("Service not found");

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
        }
    }
}