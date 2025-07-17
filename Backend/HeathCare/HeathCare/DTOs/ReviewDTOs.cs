// DTOs/ReviewDTOs.cs
using System.ComponentModel.DataAnnotations;

namespace HeathCare.DTOs
{
    public class ReviewDTO
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public string VisitorName { get; set; }
        public string VisitorEmail { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
    }

    public class ReviewCreateDTO
    {
        [Required]
        [StringLength(500, MinimumLength = 10)]
        public string Content { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        [StringLength(100)]
        public string VisitorName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string VisitorEmail { get; set; }
    }

    public class ReviewUpdateDTO
    {
        public int Id { get; set; }

        [StringLength(500, MinimumLength = 10)]
        public string Content { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }
    }
}