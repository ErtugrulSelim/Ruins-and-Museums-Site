using Microsoft.EntityFrameworkCore;
using RuinsandMuseums.Models;

namespace RuinsandMuseums.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Ruin> Ruins { get; set; }
        public DbSet<Museum> Museums { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Ruin>()
                .HasOne(r => r.Category)
                .WithMany(c => c.Ruins)
                .HasForeignKey(r => r.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Museum>()
                .HasOne(m => m.Category)
                .WithMany(c => c.Museums)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Ancient Ruins", Description = "Historical sites from ancient civilizations" },
                new Category { Id = 2, Name = "Archaeological Sites", Description = "Sites of archaeological importance" },
                new Category { Id = 3, Name = "Historical Museums", Description = "Museums showcasing historical artifacts" },
                new Category { Id = 4, Name = "Art Museums", Description = "Museums dedicated to art and culture" },
                new Category { Id = 5, Name = "Natural History Museums", Description = "Museums focusing on natural history and science" }
            );
        }
    }
} 