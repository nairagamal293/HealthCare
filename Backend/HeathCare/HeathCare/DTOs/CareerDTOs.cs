using System.ComponentModel.DataAnnotations;

namespace HeathCare.DTOs
{
    public class CareerDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string EmploymentType { get; set; }
        public DateTime PostedDate { get; set; }
        public DateTime? ClosingDate { get; set; }
        public bool IsActive { get; set; }
        public string Requirements { get; set; }
        public string Responsibilities { get; set; }
    }

    public class CareerCreateDTO
    {
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
        public string EmploymentType { get; set; }

        public DateTime? ClosingDate { get; set; }
        public string Requirements { get; set; }
        public string Responsibilities { get; set; }
    }

    public class CareerUpdateDTO
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
        public string EmploymentType { get; set; }

        public DateTime? ClosingDate { get; set; }
        public bool IsActive { get; set; }
        public string Requirements { get; set; }
        public string Responsibilities { get; set; }
    }
}
