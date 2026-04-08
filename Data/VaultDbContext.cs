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
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<MaintenanceItem> MaintenanceItems => Set<MaintenanceItem>();
    public DbSet<MaintenanceLog> MaintenanceLogs => Set<MaintenanceLog>();
    public DbSet<Note> Notes => Set<Note>();
    public DbSet<BudgetCategory> BudgetCategories => Set<BudgetCategory>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<ShoppingList> ShoppingLists => Set<ShoppingList>();
    public DbSet<ShoppingItem> ShoppingItems => Set<ShoppingItem>();
    public DbSet<Person> Persons => Set<Person>();
    public DbSet<Chore> Chores => Set<Chore>();
    public DbSet<ChoreLog> ChoreLogs => Set<ChoreLog>();
    public DbSet<HealthMetric> HealthMetrics => Set<HealthMetric>();
    public DbSet<PantryItem> PantryItems => Set<PantryItem>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<MealPlan> MealPlans => Set<MealPlan>();
    public DbSet<Meter> Meters => Set<Meter>();
    public DbSet<MeterReading> MeterReadings => Set<MeterReading>();
    public DbSet<CalendarEvent> CalendarEvents => Set<CalendarEvent>();
    public DbSet<XpEvent> XpEvents => Set<XpEvent>();
    public DbSet<Challenge> Challenges => Set<Challenge>();
    public DbSet<ChallengeProgress> ChallengeProgressEntries => Set<ChallengeProgress>();
    public DbSet<Template> Templates => Set<Template>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // UserProfile
        modelBuilder.Entity<UserProfile>(e =>
        {
            e.ToTable("UserProfiles");
            e.HasKey(u => u.Id);
            e.Property(u => u.Name).IsRequired().HasMaxLength(100);
        });

        // Habit
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
            e.HasOne(h => h.User).WithMany().HasForeignKey(h => h.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        // HabitEntry
        modelBuilder.Entity<HabitEntry>(e =>
        {
            e.ToTable("HabitEntries");
            e.HasKey(he => he.Id);
            e.HasIndex(he => new { he.HabitId, he.Date }).IsUnique();
            e.HasIndex(he => new { he.UserId, he.Date });
            e.HasOne(he => he.Habit).WithMany(h => h.Entries).HasForeignKey(he => he.HabitId).OnDelete(DeleteBehavior.Cascade);
        });

        // TaskColumn
        modelBuilder.Entity<TaskColumn>(e =>
        {
            e.ToTable("TaskColumns");
            e.HasKey(c => c.Id);
            e.Property(c => c.Name).IsRequired().HasMaxLength(100);
            e.Property(c => c.Color).IsRequired().HasMaxLength(50);
            e.HasIndex(c => new { c.UserId, c.DeletedAt });
            e.HasQueryFilter(c => c.DeletedAt == null);
            e.HasOne(c => c.User).WithMany().HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        // TaskItem
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
            e.HasOne(t => t.Column).WithMany(c => c.Tasks).HasForeignKey(t => t.ColumnId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(t => t.User).WithMany().HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(t => t.ProjectNav).WithMany(p => p.Tasks).HasForeignKey(t => t.ProjectId).OnDelete(DeleteBehavior.SetNull).IsRequired(false);
        });

        // Project
        modelBuilder.Entity<Project>(e =>
        {
            e.ToTable("Projects");
            e.HasKey(p => p.Id);
            e.Property(p => p.Name).IsRequired().HasMaxLength(200);
            e.Property(p => p.Description).HasMaxLength(2000);
            e.Property(p => p.Status).IsRequired().HasMaxLength(20);
            e.Property(p => p.Color).HasMaxLength(50);
            e.HasIndex(p => new { p.UserId, p.DeletedAt });
            e.HasQueryFilter(p => p.DeletedAt == null);
            e.HasOne(p => p.User).WithMany().HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        // MaintenanceItem
        modelBuilder.Entity<MaintenanceItem>(e =>
        {
            e.ToTable("MaintenanceItems");
            e.HasKey(m => m.Id);
            e.Property(m => m.Name).IsRequired().HasMaxLength(200);
            e.Property(m => m.Category).HasMaxLength(100);
            e.Property(m => m.Room).HasMaxLength(100);
            e.Property(m => m.Icon).HasMaxLength(10);
            e.Property(m => m.Notes).HasMaxLength(500);
            e.HasIndex(m => new { m.UserId, m.DeletedAt });
            e.HasQueryFilter(m => m.DeletedAt == null);
            e.HasOne(m => m.User).WithMany().HasForeignKey(m => m.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        // MaintenanceLog
        modelBuilder.Entity<MaintenanceLog>(e =>
        {
            e.ToTable("MaintenanceLogs");
            e.HasKey(l => l.Id);
            e.Property(l => l.Note).HasMaxLength(500);
            e.Property(l => l.Cost).HasColumnType("decimal(10,2)");
            e.HasOne(l => l.Item).WithMany(m => m.Logs).HasForeignKey(l => l.ItemId).OnDelete(DeleteBehavior.Cascade);
        });

        // Note
        modelBuilder.Entity<Note>(e =>
        {
            e.ToTable("Notes");
            e.HasKey(n => n.Id);
            e.Property(n => n.Title).IsRequired().HasMaxLength(300);
            e.Property(n => n.Color).HasMaxLength(50);
            e.Property(n => n.Tags).HasMaxLength(500);
            e.HasIndex(n => new { n.UserId, n.DeletedAt });
            e.HasQueryFilter(n => n.DeletedAt == null);
            e.HasOne(n => n.User).WithMany().HasForeignKey(n => n.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        // BudgetCategory
        modelBuilder.Entity<BudgetCategory>(e =>
        {
            e.ToTable("BudgetCategories");
            e.HasKey(b => b.Id);
            e.Property(b => b.Name).IsRequired().HasMaxLength(100);
            e.Property(b => b.MonthlyLimit).HasColumnType("decimal(10,2)");
            e.Property(b => b.Color).HasMaxLength(50);
            e.Property(b => b.Icon).HasMaxLength(10);
            e.HasIndex(b => new { b.UserId, b.DeletedAt });
            e.HasQueryFilter(b => b.DeletedAt == null);
            e.HasOne(b => b.User).WithMany().HasForeignKey(b => b.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        // Transaction
        modelBuilder.Entity<Transaction>(e =>
        {
            e.ToTable("Transactions");
            e.HasKey(t => t.Id);
            e.Property(t => t.Amount).HasColumnType("decimal(10,2)");
            e.Property(t => t.Description).HasMaxLength(300);
            e.Property(t => t.Payee).HasMaxLength(200);
            e.Property(t => t.RecurrenceRule).HasMaxLength(50);
            e.HasIndex(t => new { t.UserId, t.DeletedAt });
            e.HasIndex(t => new { t.UserId, t.Date });
            e.HasQueryFilter(t => t.DeletedAt == null);
            e.HasOne(t => t.Category).WithMany(c => c.Transactions).HasForeignKey(t => t.CategoryId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(t => t.User).WithMany().HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        // ShoppingList
        modelBuilder.Entity<ShoppingList>(e =>
        {
            e.ToTable("ShoppingLists");
            e.HasKey(s => s.Id);
            e.Property(s => s.Name).IsRequired().HasMaxLength(200);
            e.Property(s => s.Color).HasMaxLength(50);
            e.HasIndex(s => new { s.UserId, s.DeletedAt });
            e.HasQueryFilter(s => s.DeletedAt == null);
            e.HasOne(s => s.User).WithMany().HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        // ShoppingItem
        modelBuilder.Entity<ShoppingItem>(e =>
        {
            e.ToTable("ShoppingItems");
            e.HasKey(i => i.Id);
            e.Property(i => i.Name).IsRequired().HasMaxLength(200);
            e.Property(i => i.Unit).HasMaxLength(50);
            e.Property(i => i.Category).HasMaxLength(100);
            e.Property(i => i.Note).HasMaxLength(300);
            e.Property(i => i.Quantity).HasColumnType("decimal(10,3)");
            e.HasIndex(i => new { i.ListId, i.DeletedAt });
            e.HasQueryFilter(i => i.DeletedAt == null);
            e.HasOne(i => i.List).WithMany(l => l.Items).HasForeignKey(i => i.ListId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(i => i.User).WithMany().HasForeignKey(i => i.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        // Person (Birthdays/Anniversaries)
        modelBuilder.Entity<Person>(e =>
        {
            e.ToTable("Persons");
            e.HasKey(p => p.Id);
            e.Property(p => p.Name).IsRequired().HasMaxLength(200);
            e.Property(p => p.EventType).IsRequired().HasMaxLength(20);
            e.Property(p => p.Category).HasMaxLength(50);
            e.Property(p => p.Notes).HasMaxLength(500);
            e.Property(p => p.ReminderDays).HasMaxLength(50);
            e.HasIndex(p => new { p.UserId, p.DeletedAt });
            e.HasQueryFilter(p => p.DeletedAt == null);
            e.HasOne(p => p.User).WithMany().HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        // Chore
        modelBuilder.Entity<Chore>(e =>
        {
            e.ToTable("Chores");
            e.HasKey(c => c.Id);
            e.Property(c => c.Name).IsRequired().HasMaxLength(200);
            e.Property(c => c.AssignedTo).IsRequired().HasMaxLength(100);
            e.Property(c => c.Frequency).IsRequired().HasMaxLength(20);
            e.Property(c => c.Room).HasMaxLength(100);
            e.Property(c => c.RotationType).IsRequired().HasMaxLength(20);
            e.HasIndex(c => new { c.UserId, c.DeletedAt });
            e.HasQueryFilter(c => c.DeletedAt == null);
            e.HasOne(c => c.User).WithMany().HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        // ChoreLog
        modelBuilder.Entity<ChoreLog>(e =>
        {
            e.ToTable("ChoreLogs");
            e.HasKey(l => l.Id);
            e.Property(l => l.CompletedBy).IsRequired().HasMaxLength(100);
            e.Property(l => l.Note).HasMaxLength(300);
            e.HasIndex(l => new { l.ChoreId, l.CompletedAt });
            e.HasOne(l => l.Chore).WithMany(c => c.Logs).HasForeignKey(l => l.ChoreId).OnDelete(DeleteBehavior.Cascade);
        });

        // HealthMetric
        modelBuilder.Entity<HealthMetric>(e =>
        {
            e.ToTable("HealthMetrics");
            e.HasKey(h => h.Id);
            e.Property(h => h.MetricType).IsRequired().HasMaxLength(20);
            e.Property(h => h.Unit).IsRequired().HasMaxLength(20);
            e.Property(h => h.Note).HasMaxLength(300);
            e.HasIndex(h => new { h.UserId, h.MetricType, h.Date }).IsUnique();
            e.HasOne(h => h.User).WithMany().HasForeignKey(h => h.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        // PantryItem
        modelBuilder.Entity<PantryItem>(e =>
        {
            e.ToTable("PantryItems");
            e.HasKey(p => p.Id);
            e.Property(p => p.Name).IsRequired().HasMaxLength(200);
            e.Property(p => p.Category).IsRequired().HasMaxLength(100);
            e.Property(p => p.Location).IsRequired().HasMaxLength(50);
            e.Property(p => p.Unit).HasMaxLength(50);
            e.Property(p => p.Note).HasMaxLength(300);
            e.Property(p => p.Quantity).HasColumnType("decimal(10,3)");
            e.Property(p => p.MinStock).HasColumnType("decimal(10,3)");
            e.HasIndex(p => new { p.UserId, p.DeletedAt });
            e.HasQueryFilter(p => p.DeletedAt == null);
            e.HasOne(p => p.User).WithMany().HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        // Recipe
        modelBuilder.Entity<Recipe>(e =>
        {
            e.ToTable("Recipes");
            e.HasKey(r => r.Id);
            e.Property(r => r.Name).IsRequired().HasMaxLength(200);
            e.Property(r => r.Tags).HasMaxLength(500);
            e.Property(r => r.ImageUrl).HasMaxLength(300);
            e.Property(r => r.SourceUrl).HasMaxLength(300);
            e.HasIndex(r => new { r.UserId, r.DeletedAt });
            e.HasQueryFilter(r => r.DeletedAt == null);
            e.HasOne(r => r.User).WithMany().HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        // MealPlan
        modelBuilder.Entity<MealPlan>(e =>
        {
            e.ToTable("MealPlans");
            e.HasKey(m => m.Id);
            e.Property(m => m.Slot).IsRequired().HasMaxLength(20);
            e.Property(m => m.FreeText).HasMaxLength(200);
            e.HasIndex(m => new { m.UserId, m.Date, m.Slot }).IsUnique();
            e.HasOne(m => m.Recipe).WithMany().HasForeignKey(m => m.RecipeId).OnDelete(DeleteBehavior.SetNull).IsRequired(false);
            e.HasOne(m => m.User).WithMany().HasForeignKey(m => m.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        // Meter
        modelBuilder.Entity<Meter>(e =>
        {
            e.ToTable("Meters");
            e.HasKey(m => m.Id);
            e.Property(m => m.Name).IsRequired().HasMaxLength(100);
            e.Property(m => m.MeterType).IsRequired().HasMaxLength(30);
            e.Property(m => m.Unit).IsRequired().HasMaxLength(20);
            e.Property(m => m.TariffRate).HasColumnType("decimal(10,4)");
            e.Property(m => m.CurrentReading).HasColumnType("decimal(12,3)");
            e.HasIndex(m => new { m.UserId, m.DeletedAt });
            e.HasQueryFilter(m => m.DeletedAt == null);
            e.HasOne(m => m.User).WithMany().HasForeignKey(m => m.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        // MeterReading
        modelBuilder.Entity<MeterReading>(e =>
        {
            e.ToTable("MeterReadings");
            e.HasKey(r => r.Id);
            e.Property(r => r.Value).HasColumnType("decimal(12,3)");
            e.Property(r => r.Note).HasMaxLength(300);
            e.HasIndex(r => new { r.MeterId, r.ReadingDate });
            e.HasOne(r => r.Meter).WithMany(m => m.Readings).HasForeignKey(r => r.MeterId).OnDelete(DeleteBehavior.Cascade);
        });

        // CalendarEvent
        modelBuilder.Entity<CalendarEvent>(e =>
        {
            e.ToTable("CalendarEvents");
            e.HasKey(c => c.Id);
            e.Property(c => c.Title).IsRequired().HasMaxLength(300);
            e.Property(c => c.Color).HasMaxLength(50);
            e.Property(c => c.Description).HasMaxLength(500);
            e.Property(c => c.LinkedModule).HasMaxLength(20);
            e.Property(c => c.RecurrenceRule).HasMaxLength(100);
            e.HasIndex(c => new { c.UserId, c.Date });
            e.HasQueryFilter(c => c.DeletedAt == null);
            e.HasOne(c => c.User).WithMany().HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        // XpEvent
        modelBuilder.Entity<XpEvent>(e =>
        {
            e.ToTable("XpEvents");
            e.HasKey(x => x.Id);
            e.Property(x => x.Action).IsRequired().HasMaxLength(50);
            e.Property(x => x.LinkedModule).HasMaxLength(20);
            e.HasIndex(x => new { x.UserId, x.CreatedAt });
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        // Challenge
        modelBuilder.Entity<Challenge>(e =>
        {
            e.ToTable("Challenges");
            e.HasKey(c => c.Id);
            e.Property(c => c.Title).IsRequired().HasMaxLength(200);
            e.Property(c => c.Description).HasMaxLength(500);
            e.Property(c => c.TargetModule).IsRequired().HasMaxLength(20);
            e.HasQueryFilter(c => c.DeletedAt == null);
            e.HasOne(c => c.CreatedByUser).WithMany().HasForeignKey(c => c.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
        });

        // ChallengeProgress
        modelBuilder.Entity<ChallengeProgress>(e =>
        {
            e.ToTable("ChallengeProgress");
            e.HasKey(p => p.Id);
            e.HasIndex(p => new { p.ChallengeId, p.UserId }).IsUnique();
            e.HasOne(p => p.Challenge).WithMany(c => c.Progress).HasForeignKey(p => p.ChallengeId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(p => p.User).WithMany().HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        // Template
        modelBuilder.Entity<Template>(e =>
        {
            e.ToTable("Templates");
            e.HasKey(t => t.Id);
            e.Property(t => t.Name).IsRequired().HasMaxLength(200);
            e.Property(t => t.Description).HasMaxLength(500);
            e.Property(t => t.Category).IsRequired().HasMaxLength(50);
            e.Property(t => t.Icon).HasMaxLength(10);
        });
    }
}
