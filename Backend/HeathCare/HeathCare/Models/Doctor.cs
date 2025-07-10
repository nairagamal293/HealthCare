namespace HeathCare.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Specialty { get; set; }
        public string Bio { get; set; }
        public string ImagePath { get; set; } // Changed from ImageUrl to ImagePath
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}
