using System.ComponentModel.DataAnnotations;

namespace HeathCare.DTOs
{
    // DTOs/BlogDTOs.cs
    public class BlogDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImagePath { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class BlogCreateDTO
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 200 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required")]
        [StringLength(5000, ErrorMessage = "Content cannot exceed 5000 characters")]
        public string Content { get; set; }

        public IFormFile ImageFile { get; set; }
    }

    public class BlogUpdateDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 200 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required")]
        [StringLength(5000, ErrorMessage = "Content cannot exceed 5000 characters")]
        public string Content { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}
