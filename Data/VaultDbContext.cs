using Microsoft.EntityFrameworkCore;
using Vault.Data.Models;

namespace Vault.Data;

public sealed class VaultDbContext(DbContextOptions<VaultDbContext> options) : DbContext(options)
{
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Habit> Habits => Set<Habit>();
    public DbSet<HabitEntry> HabitEntries => Set<HabitEntry>();
    public DbSet<TaskColumn> TaskColumns => Set<TaskColumn>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserProfile>(e =>
        {
            e.ToTable("UserProfiles");
            e.HasKey(u => u.Id);
            e.Property(u => u.Name).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Habit>(e =>
        {
            e.ToTable("Habits");
            e.HasKey(h => h.Id);
            e.Property(h => h.Name).IsRequired().HasMaxLength(200);
            e.Property(h => h.Type).IsRequired().HasMaxLength(20);
            e.Property(h => h.Frequency).IsRequired().HasMaxLength(50);
            e.Property(h => h.Color).IsRequired().HasMaxLength(50);
            e.Property(h => h.HexColor).IsRequired().HasMaxLength(10);
            e.Property(h => h.UnitOfMeasure).HasMaxLength(20);
            e.Property(h => h.Icon).HasMaxLength(10);
            e.Property(h => h.Notes).HasMaxLength(500);
            e.HasIndex(h => new { h.UserId, h.DeletedAt });
            e.HasQueryFilter(h => h.DeletedAt == null);
            e.HasOne(h => h.User)
             .WithMany()
             .HasForeignKey(h => h.UserId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<HabitEntry>(e =>
        {
            e.ToTable("HabitEntries");
            e.HasKey(he => he.Id);
            e.HasIndex(he => new { he.HabitId, he.Date }).IsUnique();
            e.HasIndex(he => new { he.UserId, he.Date });
            e.HasOne(he => he.Habit)
             .WithMany(h => h.Entries)
             .HasForeignKey(he => he.HabitId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TaskColumn>(e =>
        {
            e.ToTable("TaskColumns");
            e.HasKey(c => c.Id);
            e.Property(c => c.Name).IsRequired().HasMaxLength(100);
            e.Property(c => c.Color).IsRequired().HasMaxLength(50);
            e.HasIndex(c => new { c.UserId, c.DeletedAt });
            e.HasQueryFilter(c => c.DeletedAt == null);
            e.HasOne(c => c.User)
             .WithMany()
             .HasForeignKey(c => c.UserId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TaskItem>(e =>
        {
            e.ToTable("TaskItems");
            e.HasKey(t => t.Id);
            e.Property(t => t.Title).IsRequired().HasMaxLength(300);
            e.Property(t => t.Description).HasMaxLength(2000);
            e.Property(t => t.Priority).IsRequired().HasMaxLength(10);
            e.Property(t => t.Project).HasMaxLength(200);
            e.HasIndex(t => new { t.UserId, t.DeletedAt });
            e.HasIndex(t => new { t.ColumnId, t.Position });
            e.HasQueryFilter(t => t.DeletedAt == null);
            e.HasOne(t => t.Column)
             .WithMany(c => c.Tasks)
             .HasForeignKey(t => t.ColumnId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(t => t.User)
             .WithMany()
             .HasForeignKey(t => t.UserId)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
