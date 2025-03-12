using Microsoft.EntityFrameworkCore;
using MyBlogDotnet.Models;

namespace MyBlogDotnet.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational: EnableLegacyMigrations", false);
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Post>().ToTable("posts");
            // base.OnModelCreating(modelBuilder);
        }
    }
}