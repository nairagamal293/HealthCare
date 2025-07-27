using System.ComponentModel.DataAnnotations;

namespace HeathCare.Models
{
    public class Career
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        [StringLength(100)]
        public string Location { get; set; }

        [Required]
        [StringLength(50)]
        public string EmploymentType { get; set; } // Full-time, Part-time, Contract

        public DateTime PostedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ClosingDate { get; set; }
        public bool IsActive { get; set; } = true;

        [StringLength(200)]
        public string Requirements { get; set; }

        [StringLength(200)]
        public string Responsibilities { get; set; }
    }
}
