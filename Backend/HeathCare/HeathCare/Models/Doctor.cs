// Models/Doctor.cs
using HeathCare.Models.HeathCare.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace HeathCare.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Specialty { get; set; }
        public string Bio { get; set; }
        public string ImagePath { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        // Simple availability properties (optional)
        public string Availability { get; set; } // e.g., "Mon-Fri: 9am-5pm"
        public string WorkingDays { get; set; }   // e.g., "Monday, Wednesday, Friday"
        public string WorkingHours { get; set; }  // e.g., "9:00 AM - 5:00 PM"

        // Navigation property for detailed availabilities
        public ICollection<DoctorAvailability> Availabilities { get; set; } = new List<DoctorAvailability>();

        // Reviews
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        // Calculated properties (not stored in DB)
        [NotMapped]
        public double? AverageRating => Reviews?.Any() == true ? Reviews.Average(r => r.Rating) : null;

        [NotMapped]
        public int ReviewCount => Reviews?.Count ?? 0;
    }
}