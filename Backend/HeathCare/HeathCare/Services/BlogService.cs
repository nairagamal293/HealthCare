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

        public BlogService(
            ApplicationDbContext context,
            IMapper mapper,
            IFileService fileService)
        {
            _context = context;
            _mapper = mapper;
            _fileService = fileService;
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
            // Save the image file
            var imagePath = await _fileService.SaveImageAsync(blogDto.ImageFile);

            var blog = _mapper.Map<Blog>(blogDto);
            blog.ImagePath = imagePath;
            blog.CreatedAt = DateTime.UtcNow;

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
                blog.ImagePath = await _fileService.SaveImageAsync(blogDto.ImageFile);
            }

            _mapper.Map(blogDto, blog);
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