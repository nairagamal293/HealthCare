using HeathCare.Models.HeathCare.Models;
using System.ComponentModel.DataAnnotations;

namespace HeathCare.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }


        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        [Required]
        public DateTime PreferredDate { get; set; }

        public string Message { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsConfirmed { get; set; } = false;
    }
}
