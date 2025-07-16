using System.ComponentModel.DataAnnotations;

namespace HeathCare.DTOs
{
    // DTOs/DepartmentDTOs.cs
    using System.ComponentModel.DataAnnotations;

    namespace HeathCare.DTOs
    {
        public class DepartmentDTO
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string ImagePath { get; set; }
            public int DoctorCount { get; set; }
            public List<ServiceDTO> Services { get; set; } = new List<ServiceDTO>();
        }

        public class DepartmentCreateDTO
        {
            [Required]
            [StringLength(100, MinimumLength = 2)]
            public string Name { get; set; }

            [Required]
            [StringLength(500)]
            public string Description { get; set; }

            public IFormFile ImageFile { get; set; }
            public List<ServiceCreateDTO> Services { get; set; } = new List<ServiceCreateDTO>();
        }

        public class DepartmentUpdateDTO
        {
            public int Id { get; set; }

            [Required]
            [StringLength(100, MinimumLength = 2)]
            public string Name { get; set; }

            [Required]
            [StringLength(500)]
            public string Description { get; set; }

            public IFormFile? ImageFile { get; set; }
            public List<ServiceCreateDTO> Services { get; set; } = new List<ServiceCreateDTO>();
        }
    }
}
