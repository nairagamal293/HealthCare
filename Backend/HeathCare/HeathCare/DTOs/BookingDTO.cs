using System.ComponentModel.DataAnnotations;

namespace HeathCare.DTOs
{
    // DTOs/BookingDTOs.cs
    public class BookingDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public DateTime PreferredDate { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsConfirmed { get; set; }
    }

    public class BookingCreateDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string FullName { get; set; }

        [Required]
        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int DepartmentId { get; set; }

        [Required]
        [FutureDate(ErrorMessage = "Preferred date must be in the future")]
        public DateTime PreferredDate { get; set; }

        [StringLength(500)]
        public string Message { get; set; }
    }

    public class BookingUpdateDTO
    {
        public int Id { get; set; }
        public bool IsConfirmed { get; set; }
    }

    // Custom validation attribute for future date
    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return value is DateTime date && date > DateTime.Now;
        }
    }
}
