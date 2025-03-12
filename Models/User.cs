using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBlogDotnet.Models
{
    public class User
    {
        [Key]
        [Column("id", TypeName = "VARCHAR(50)")]
        public string Id { get; set; }

        [Column("username", TypeName = "VARCHAR(50)")]
        public string Username { get; set; }

        [Column("email", TypeName = "VARCHAR(100)")]
        public string Email { get; set; }

        [Column("password_hash", TypeName = "VARCHAR(255)")]
        public string PasswordHash { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}