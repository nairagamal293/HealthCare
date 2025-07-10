using System.ComponentModel.DataAnnotations;

namespace HeathCare.DTOs
{
    // DTOs/BlogDTOs.cs
    public class BlogDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class BlogCreateDTO
    {
        [Required]
        [StringLength(200, MinimumLength = 5)]
        public string Title { get; set; }

        [Required]
        [StringLength(5000)]
        public string Content { get; set; }

        [Required]
        public IFormFile ImageFile { get; set; } // Changed from ImageUrl to ImageFile
    }

    // DTOs/BlogDTOs.cs
    public class BlogUpdateDTO
    {
        public int Id { get; set; } // Add this line

        [Required]
        [StringLength(200, MinimumLength = 5)]
        public string Title { get; set; }

        [Required]
        [StringLength(5000)]
        public string Content { get; set; }

        public IFormFile? ImageFile { get; set; } // Make nullable if not always required
    }
}
