using AutoMapper;
using HeathCare.DTOs;
using HeathCare.Models;

namespace HeathCare
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Department mapping
            CreateMap<Department, DepartmentDTO>()
                .ForMember(dest => dest.DoctorCount,
                           opt => opt.MapFrom(src => src.Doctors.Count));

            // Doctor mappings
            CreateMap<Doctor, DoctorDTO>()
                .ForMember(dest => dest.DepartmentName,
                           opt => opt.MapFrom(src => src.Department.Name));

            CreateMap<DoctorCreateDTO, Doctor>();
            CreateMap<DoctorUpdateDTO, Doctor>();

            // Add other mappings as needed
            CreateMap<DepartmentCreateDTO, Department>();
            CreateMap<DepartmentUpdateDTO, Department>();
        }
    }
}