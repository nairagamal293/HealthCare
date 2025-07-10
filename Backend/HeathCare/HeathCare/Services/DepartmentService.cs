using AutoMapper;
using HealthCare.Data;
using HeathCare.DTOs;
using HeathCare.Models;
using Microsoft.EntityFrameworkCore;

namespace HeathCare.Services
{
    // Services/DepartmentService.cs
    public class DepartmentService : IDepartmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public DepartmentService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DepartmentDTO>> GetAllDepartmentsAsync()
        {
            var departments = await _context.Departments
                .Include(d => d.Doctors) // Make sure to include Doctors
                .ToListAsync();

            return _mapper.Map<IEnumerable<DepartmentDTO>>(departments);
        }


        public async Task<DepartmentDTO> GetDepartmentByIdAsync(int id)
        {
            var department = await _context.Departments
                .Include(d => d.Doctors)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (department == null) return null;

            var departmentDTO = _mapper.Map<DepartmentDTO>(department);
            departmentDTO.DoctorCount = department.Doctors?.Count ?? 0;
            return departmentDTO;
        }

        public async Task<DepartmentDTO> CreateDepartmentAsync(DepartmentCreateDTO departmentDto)
        {
            var department = _mapper.Map<Department>(departmentDto);

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            return _mapper.Map<DepartmentDTO>(department);
        }

        public async Task UpdateDepartmentAsync(int id, DepartmentUpdateDTO departmentDto)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null) throw new KeyNotFoundException("Department not found");

            _mapper.Map(departmentDto, department);
            _context.Departments.Update(department);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDepartmentAsync(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null) throw new KeyNotFoundException("Department not found");

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
        }
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
