using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace HeathCare.DTOs
{
    // DTOs/DoctorDTOs.cs
    // DTOs/DoctorDTO.cs
    public class DoctorDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Specialty { get; set; }
        public string Bio { get; set; }
        public string ImagePath { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }

        // Simple availability properties
        public string Availability { get; set; }
        public string WorkingDays { get; set; }
        public string WorkingHours { get; set; }

        // Review-related properties
        public double? AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public List<ReviewDTO> Reviews { get; set; } = new List<ReviewDTO>();

        // Detailed availabilities
        public List<DoctorAvailabilityDTO> Availabilities { get; set; } = new List<DoctorAvailabilityDTO>();
    }

    public class DoctorCreateDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Specialty { get; set; }

        [StringLength(500)]
        public string Bio { get; set; }

        public IFormFile ImageFile { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int DepartmentId { get; set; }

        public string WorkingDays { get; set; }
        public string Availability { get; set; }
        public string WorkingHours { get; set; }

        public string AvailabilitiesJson { get; set; }

        [NotMapped]
        public List<DoctorAvailabilityCreateDTO> Availabilities =>
            string.IsNullOrEmpty(AvailabilitiesJson)
                ? new List<DoctorAvailabilityCreateDTO>()
                : JsonSerializer.Deserialize<List<DoctorAvailabilityCreateDTO>>(AvailabilitiesJson);
    }

    public class DoctorUpdateDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Specialty { get; set; }

        [StringLength(500)]
        public string Bio { get; set; }

        public IFormFile? ImageFile { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int DepartmentId { get; set; }

        // Add these new properties
        public string Availability { get; set; }
        public string WorkingDays { get; set; }
        public string WorkingHours { get; set; }

        // Add for availability updates
        public string AvailabilitiesJson { get; set; }
    }
}
