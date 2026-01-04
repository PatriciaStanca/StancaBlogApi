using Microsoft.EntityFrameworkCore;
using StancaBlogApi.Models;

namespace StancaBlogApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Comment → User (NO cascade to avoid multiple cascade paths)
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Comment → BlogPost (cascade is OK here)
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.BlogPost)
            .WithMany(b => b.Comments)
            .HasForeignKey(c => c.BlogPostId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed categories (fixed valid values)
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Mindfulness & Meditation" },
            new Category { Id = 2, Name = "Personal Growth" },
            new Category { Id = 3, Name = "Lifestyle & Balance" },
            new Category { Id = 4, Name = "Creativity & Inspiration" }
        );
    }
}
