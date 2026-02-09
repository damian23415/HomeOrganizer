using HomeOrganizer.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace HomeOrganizer.Infrastructure.Data;

public class HomeOrganizerDbContext : DbContext
{
  public HomeOrganizerDbContext(DbContextOptions<HomeOrganizerDbContext> options) : base(options) { }

  public DbSet<User> Users { get; set; } = null!;

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<User>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(u => u.Email).HasConversion(e => e.Value, v => new(v));
      entity.Property(u => u.PasswordHash).HasMaxLength(500);
    });
  }
}