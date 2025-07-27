using System.ComponentModel.DataAnnotations;

namespace HeathCare.Models
{
    public class JobApplication
    {
        public int Id { get; set; }

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
        public int CareerId { get; set; }
        public Career Career { get; set; }

        [Required]
        [Range(0, 50)]
        public int YearsOfExperience { get; set; }

        [Required]
        [StringLength(500)]
        public string Skills { get; set; }

        [Required]
        public string CvPath { get; set; }

        public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;
        public bool IsReviewed { get; set; } = false;
    }
}
