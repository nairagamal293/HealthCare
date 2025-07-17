using HeathCare.Models.HeathCare.Models;

namespace HeathCare.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string VisitorName { get; set; }
        public string VisitorEmail { get; set; }

        // Foreign key
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
    }
}
