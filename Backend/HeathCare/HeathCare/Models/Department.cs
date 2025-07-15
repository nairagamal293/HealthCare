namespace HeathCare.Models
{
    namespace HeathCare.Models
    {
        public class Department
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string ImagePath { get; set; } // Add this line
            public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
        }
    }
}
