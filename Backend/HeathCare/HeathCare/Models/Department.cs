namespace HeathCare.Models
{
    namespace HeathCare.Models
    {
        public class Department
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string ImagePath { get; set; }
            public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
            public ICollection<Service> Services { get; set; } = new List<Service>();
        }
    }
}