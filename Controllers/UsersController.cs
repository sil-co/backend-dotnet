using Microsoft.AspNetCore.Mvc;
using MyBlogDotnet.Data;
using MyBlogDotnet.Models;
using Microsoft.EntityFrameworkCore;
using MyBlogDotnet.Utils;
using Microsoft.AspNetCore.Identity.Data;

namespace MyBlogDotnet.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtUtil _jwtUtil;

        public UsersController(AppDbContext context, JwtUtil jwtUtil) {
            _context = context;
            _jwtUtil = jwtUtil;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            var dbUser = _context.Users.FirstOrDefault(u => u.Email == loginRequest.Email);
            if (dbUser == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, dbUser.PasswordHash))
            {
                return Unauthorized("Invalid credentials");
            }
            string token = _jwtUtil.GenerateToken(dbUser.Email, dbUser.Id);
            return Ok(token);
        }
    }
}