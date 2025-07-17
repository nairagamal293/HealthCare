// Models/Doctor.cs
using HeathCare.Models.HeathCare.Models;
using System.Collections.Generic;

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

        // Change from ReviewDTO to Review
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        // Calculated properties (not stored in DB)
        public double? AverageRating => Reviews?.Any() == true ? Reviews.Average(r => r.Rating) : null;
        public int ReviewCount => Reviews?.Count ?? 0;
    }
}