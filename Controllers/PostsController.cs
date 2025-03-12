using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBlogDotnet.Data;
using MyBlogDotnet.DTOs;
using MyBlogDotnet.Models;
using MyBlogDotnet.Utils;

namespace MyBlogDotnet.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtUtil _jwtUtil;

        public PostsController(AppDbContext context, JwtUtil jwtUtil)
        {
            _context = context;
            _jwtUtil = jwtUtil;
        }

        [HttpGet]
        public IActionResult GetPosts()
        {
            return Ok(_context.Posts.ToList()); 
        }

        [HttpGet("{id}")]
        public IActionResult GetPostById(string id)
        {
            var post = _context.Posts.Find(id);
            if (post == null) return NotFound();
            return Ok(post);
        }

        [HttpGet("verify-owner/{id}")]
        public IActionResult CheckPostOwnership(string id, [FromHeader] string Authorization)
        {
            var token = Authorization?.Replace("Bearer ", "");
            if (token == null) return Unauthorized();
            var claims = _jwtUtil.ValidateToken(token);
            if (claims == null) return Unauthorized(new { isOwner = false });
            var post = _context.Posts.Find(id);
            if (post == null) return NotFound(new { isOwner = false });
            var userId = claims.FindFirst("userid")?.Value;
            bool isOwner = userId == post.UserId.ToString();
            return Ok(new { isOwner });
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] BlogPostRequest blogPostRequest, [FromHeader] string Authorization)
        {
            var token = Authorization?.Replace("Bearer ", "");
            if (token == null) return Unauthorized();
            var claims = _jwtUtil.ValidateToken(token);
            if (claims == null) return Unauthorized();
            var userId = claims.FindFirst("userid")?.Value;
            if (userId == null) return Unauthorized("Invalid token");
            var newPost = new Post
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Title = blogPostRequest.Title,
                Content = blogPostRequest.Content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Posts.Add(newPost);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPostById), new { id = newPost.Id }, newPost);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(string id, [FromBody] BlogPostRequest blogPostRequest, [FromHeader] string Authorization)
        {
            var token = Authorization?.Replace("Bearer ", "");
            if (token == null) return Unauthorized();
            var claims = _jwtUtil.ValidateToken(token);
            if (claims == null) return Unauthorized();
            var userId = claims.FindFirst("userid")?.Value;
            if (userId == null) return Unauthorized("Invalid token");
            var existingPost = await _context.Posts.FindAsync(id);
            if (existingPost == null) return NotFound("Post not found");
            if (existingPost.UserId.ToString() != userId)
                return Forbid(); // 403 Forbidden if user is not the owner
            existingPost.Title = blogPostRequest.Title;
            existingPost.Content = blogPostRequest.Content;
            _context.Posts.Update(existingPost);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(string id, [FromHeader] string Authorization)
        {
            var token = Authorization?.Replace("Bearer ", "");
            if (token == null) return Unauthorized();
            var claims = _jwtUtil.ValidateToken(token);
            if (claims == null) return Unauthorized();
            var userId = claims.FindFirst("userid")?.Value;
            if (userId == null) return Unauthorized("Invalid token");
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound("Post not found");       
            if (post.UserId.ToString() != userId)
                return Forbid();
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}