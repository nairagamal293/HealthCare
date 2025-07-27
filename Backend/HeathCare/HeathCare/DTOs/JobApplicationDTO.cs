// DTOs/JobApplicationDTOs.cs
using System.ComponentModel.DataAnnotations;

namespace HeathCare.DTOs
{
    public class JobApplicationDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int CareerId { get; set; }
        public string CareerTitle { get; set; }
        public int YearsOfExperience { get; set; }
        public string Skills { get; set; }
        public string CvPath { get; set; }
        public DateTime ApplicationDate { get; set; }
        public bool IsReviewed { get; set; }
    }

    public class JobApplicationCreateDTO
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int CareerId { get; set; }

        [Required]
        [Range(0, 50)]
        public int YearsOfExperience { get; set; }

        [Required]
        [StringLength(500)]
        public string Skills { get; set; }

        [Required]
        public IFormFile CvFile { get; set; }
    }

    public class JobApplicationUpdateDTO
    {
        public int Id { get; set; }
        public bool IsReviewed { get; set; }
    }
}