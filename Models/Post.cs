using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBlogDotnet.Models
{
    public class Post
    {
        [Key]
        [Column(TypeName = "VARCHAR(50)")]
        public string Id { get; set; }

        [Required]
        [Column("user_id", TypeName = "VARCHAR(50)")]
        public string UserId { get; set; }
        
        [Column(TypeName = "VARCHAR(255)")]
        public string Title { get; set; }

        [Column(TypeName = "TEXT")]
        public string Content { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}