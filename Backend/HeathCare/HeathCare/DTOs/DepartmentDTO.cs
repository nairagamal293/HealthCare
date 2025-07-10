using System.ComponentModel.DataAnnotations;

namespace HeathCare.DTOs
{
    // DTOs/DepartmentDTOs.cs
    public class DepartmentDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public int DoctorCount { get; set; } // Make sure this property exists
    }

    public class DepartmentCreateDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Url]
        public string IconUrl { get; set; }
    }

    // DTOs/DepartmentDTOs.cs
    public class DepartmentUpdateDTO
    {
        public int Id { get; set; } // Add this line

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Url]
        public string IconUrl { get; set; }
    }
}
