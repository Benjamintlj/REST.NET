using Microsoft.EntityFrameworkCore;
using REST.NET.Entities;

namespace REST.NET.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Game> Games => Set<Game>();
    public DbSet<Genre> Genres => Set<Genre>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>().Property(g => g.Price).HasColumnType("decimal(18, 2)");

        modelBuilder.Entity<Genre>().HasData(
            new {Id = 3, Name = "fun"},
            new {Id = 4, Name = "bad"}
        );
    }
}