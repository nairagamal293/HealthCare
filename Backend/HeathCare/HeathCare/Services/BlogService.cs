// Services/BlogService.cs
using AutoMapper;
using HealthCare.Data;
using HeathCare.DTOs;
using HeathCare.Models;
using HeathCare.DTOs;
using HeathCare.Models;
using HeathCare.Services;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Services
{

    public interface IBlogService
    {
        Task<IEnumerable<BlogDTO>> GetAllBlogsAsync();
        Task<BlogDTO> GetBlogByIdAsync(int id);
        Task<BlogDTO> CreateBlogAsync(BlogCreateDTO blogDto);
        Task UpdateBlogAsync(int id, BlogUpdateDTO blogDto);
        Task DeleteBlogAsync(int id);
    }
    public class BlogService : IBlogService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        private readonly ILogger<BlogService> _logger;

        public BlogService(
            ApplicationDbContext context,
            IMapper mapper,
            IFileService fileService,
            ILogger<BlogService> logger)
        {
            _context = context;
            _mapper = mapper;
            _fileService = fileService;
            _logger = logger;
        }

        public async Task<IEnumerable<BlogDTO>> GetAllBlogsAsync()
        {
            var blogs = await _context.Blogs
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
            return _mapper.Map<IEnumerable<BlogDTO>>(blogs);
        }

        public async Task<BlogDTO> GetBlogByIdAsync(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null) return null;

            return _mapper.Map<BlogDTO>(blog);
        }

        public async Task<BlogDTO> CreateBlogAsync(BlogCreateDTO blogDto)
        {
            // Handle file upload
            string imagePath = null;
            if (blogDto.ImageFile != null && blogDto.ImageFile.Length > 0)
            {
                try
                {
                    imagePath = await _fileService.SaveImageAsync(blogDto.ImageFile, "blogs");
                    // Ensure the path is stored as a relative path
                    imagePath = imagePath.Replace("\\", "/"); // Normalize path
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving blog image");
                    throw new Exception("Could not save image file");
                }
            }

            var blog = new Blog
            {
                Title = blogDto.Title,
                Content = blogDto.Content,
                ImagePath = imagePath,
                CreatedAt = DateTime.UtcNow
            };

            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();

            return _mapper.Map<BlogDTO>(blog);
        }

        public async Task UpdateBlogAsync(int id, BlogUpdateDTO blogDto)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null) throw new KeyNotFoundException("Blog not found");

            // Handle image update if new file is provided
            if (blogDto.ImageFile != null)
            {
                // Delete old image
                if (!string.IsNullOrEmpty(blog.ImagePath))
                    _fileService.DeleteImage(blog.ImagePath);

                // Save new image
                blog.ImagePath = await _fileService.SaveImageAsync(blogDto.ImageFile, "blogs");
            }

            // Update other properties
            blog.Title = blogDto.Title;
            blog.Content = blogDto.Content;

            _context.Blogs.Update(blog);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBlogAsync(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null) throw new KeyNotFoundException("Blog not found");

            // Delete associated image
            if (!string.IsNullOrEmpty(blog.ImagePath))
                _fileService.DeleteImage(blog.ImagePath);

            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
        }
    }
}