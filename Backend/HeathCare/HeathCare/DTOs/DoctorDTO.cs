﻿using System.ComponentModel.DataAnnotations;

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

        // Review-related properties
        public double? AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public List<ReviewDTO> Reviews { get; set; } = new List<ReviewDTO>();
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

        [Required]
        public IFormFile ImageFile { get; set; } // Changed from ImageUrl to ImageFile

        [Required]
        [Range(1, int.MaxValue)]
        public int DepartmentId { get; set; }
    }

    // DTOs/DoctorDTOs.cs
    public class DoctorUpdateDTO
    {
        public int Id { get; set; } // Add this line

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Specialty { get; set; }

        [StringLength(500)]
        public string Bio { get; set; }

        public IFormFile? ImageFile { get; set; } // Make nullable if not always required

        [Required]
        [Range(1, int.MaxValue)]
        public int DepartmentId { get; set; }
    }
}
