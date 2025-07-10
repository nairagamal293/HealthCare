namespace HeathCare.Models
{
    public class Blog
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImagePath { get; set; } // Changed from ImageUrl to ImagePath
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
