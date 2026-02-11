using HomeOrganizer.Domain.Entities.Billings;
using HomeOrganizer.Domain.Entities.Users;
using HomeOrganizer.Domain.Entities.WorkTracking;
using HomeOrganizer.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace HomeOrganizer.Infrastructure.Data;

public class HomeOrganizerDbContext : DbContext
{
  public HomeOrganizerDbContext(DbContextOptions<HomeOrganizerDbContext> options) : base(options) { }

  public DbSet<User> Users { get; set; } = null!;
  public DbSet<HourlyRate> HourlyRates { get; set; } = null!;
  public DbSet<WorkDay> WorkDays { get; set; } = null!;

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<User>(entity =>
    {
      entity.HasKey(u => u.Id);
      entity.OwnsOne(u => u.Email, e =>
      {
        e.Property(p => p.Value).HasColumnName("Email");
      });
      entity.Property(u => u.PasswordHash).HasMaxLength(500);
      entity.Property(u => u.Role).HasConversion(r => r.Value, v => UserRole.From(v));
    });
    
    modelBuilder.Entity<HourlyRate>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(h => h.Rate)
          .HasConversion(
              m => m.Value,
              v => new Money(v))
          .HasColumnType("decimal(18,2)");
      entity.HasOne<User>().WithMany().HasForeignKey(h => h.UserId);
    });

    modelBuilder.Entity<WorkDay>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.HasOne<User>().WithMany().HasForeignKey(w => w.UserId);
    });
  }
}