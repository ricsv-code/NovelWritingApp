using Microsoft.EntityFrameworkCore;
using NovelWritingApp.Shared.Models;

namespace NovelWritingAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Novel> Novels { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Character> Characters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Novel>()
                .HasMany(n => n.Chapters)
                .WithOne()
                .HasForeignKey(c => c.NovelId);

            modelBuilder.Entity<Novel>()
                .HasMany(n => n.Characters)
                .WithOne()
                .HasForeignKey(c => c.NovelId);
        }

    }
}
