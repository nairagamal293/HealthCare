using HealthCare.Services;
using HeathCare.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeathCare.Controllers
{
    // Controllers/BlogsController.cs
[Route("api/[controller]")]
[ApiController]
public class BlogsController : ControllerBase
{
    private readonly IBlogService _blogService;

    public BlogsController(IBlogService blogService)
    {
        _blogService = blogService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BlogDTO>>> GetBlogs()
    {
        var blogs = await _blogService.GetAllBlogsAsync();
        return Ok(blogs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BlogDTO>> GetBlog(int id)
    {
        var blog = await _blogService.GetBlogByIdAsync(id);
        if (blog == null) return NotFound();
        return Ok(blog);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BlogDTO>> PostBlog(BlogCreateDTO blogDto)
    {
        var blog = await _blogService.CreateBlogAsync(blogDto);
        return CreatedAtAction(nameof(GetBlog), new { id = blog.Id }, blog);
    }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutBlog(int id, [FromForm] BlogUpdateDTO blogDto)
        {
            if (id != blogDto.Id)
                return BadRequest();

            try
            {
                await _blogService.UpdateBlogAsync(id, blogDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while updating the blog" });
            }
        }

        [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteBlog(int id)
    {
        try
        {
            await _blogService.DeleteBlogAsync(id);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        
        return NoContent();
    }
}
}
