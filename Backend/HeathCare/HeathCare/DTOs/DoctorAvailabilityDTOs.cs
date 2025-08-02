using System.ComponentModel.DataAnnotations;

namespace HeathCare.DTOs
{
    public class DoctorAvailabilityDTO
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public string StartTime { get; set; } // Format: "HH:mm"
        public string EndTime { get; set; }   // Format: "HH:mm"
        public bool IsAvailable { get; set; }
    }

    public class DoctorAvailabilityCreateDTO
    {
        [Required]
        public int DoctorId { get; set; }  // Add this property

        [Required]
        public int DayOfWeek { get; set; }  // Keep as int for easier JSON handling

        [Required]
        [RegularExpression(@"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Time must be in HH:mm format")]
        public string StartTime { get; set; }

        [Required]
        [RegularExpression(@"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Time must be in HH:mm format")]
        public string EndTime { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}
