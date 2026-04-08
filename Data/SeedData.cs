using Microsoft.EntityFrameworkCore;
using Vault.Data.Models;

namespace Vault.Data;

public static class SeedData
{
    public static async Task InitializeAsync(VaultDbContext db)
    {
        await db.Database.EnsureCreatedAsync();

        if (await db.UserProfiles.AnyAsync())
            return;

        // ── Users ──────────────────────────────────────────────────────────
        var jose = new UserProfile { Id = 1, Name = "José", AvatarColor = "#1D9E75" };
        var ana  = new UserProfile { Id = 2, Name = "Ana",  AvatarColor = "#7F77DD" };
        db.UserProfiles.AddRange(jose, ana);

        // ── Habits ────────────────────────────────────────────────────────
        var habits = new List<Habit>
        {
            new() { Id = 1, UserId = 1, Name = "Drink 2.5L Water", Type = "quant",   Frequency = "Daily", Color = "var(--inf)", HexColor = "#378ADD", WeeklyGoal = 7, Goal = 2500, UnitOfMeasure = "ml",    Increment = 250, SortOrder = 1, DaysOfWeek = "0,1,2,3,4,5,6" },
            new() { Id = 2, UserId = 1, Name = "Morning Prayers",  Type = "boolean", Frequency = "Daily", Color = "var(--wn)",  HexColor = "#EF9F27", WeeklyGoal = 7, Goal = 0,    UnitOfMeasure = null,    Increment = 1,   SortOrder = 2, DaysOfWeek = "0,1,2,3,4,5,6" },
            new() { Id = 3, UserId = 1, Name = "Workout",          Type = "boolean", Frequency = "Daily", Color = "var(--ac)",  HexColor = "#1D9E75", WeeklyGoal = 5, Goal = 0,    UnitOfMeasure = null,    Increment = 1,   SortOrder = 3, DaysOfWeek = "0,1,2,3,4,5,6" },
            new() { Id = 4, UserId = 1, Name = "Study 1h",         Type = "quant",   Frequency = "Daily", Color = "var(--pu)",  HexColor = "#7F77DD", WeeklyGoal = 7, Goal = 60,   UnitOfMeasure = "min",   Increment = 5,   SortOrder = 4, DaysOfWeek = "0,1,2,3,4,5,6" },
            new() { Id = 5, UserId = 1, Name = "Walk 6k Steps",    Type = "quant",   Frequency = "Daily", Color = "var(--ac)",  HexColor = "#5BC4C4", WeeklyGoal = 7, Goal = 6000, UnitOfMeasure = "steps", Increment = 500, SortOrder = 5, DaysOfWeek = "0,1,2,3,4,5,6" },
            new() { Id = 6, UserId = 1, Name = "Evening Prayers",  Type = "boolean", Frequency = "Daily", Color = "var(--pu)",  HexColor = "#C77DD3", WeeklyGoal = 7, Goal = 0,    UnitOfMeasure = null,    Increment = 1,   SortOrder = 6, DaysOfWeek = "0,1,2,3,4,5,6" },
            new() { Id = 7, UserId = 1, Name = "Go to Bed Early",  Type = "boolean", Frequency = "Daily", Color = "var(--wn)",  HexColor = "#E07B54", WeeklyGoal = 7, Goal = 0,    UnitOfMeasure = null,    Increment = 1,   SortOrder = 7, DaysOfWeek = "0,1,2,3,4,5,6" },
        };
        db.Habits.AddRange(habits);

        // ── Task Columns ──────────────────────────────────────────────────
        var taskColumns = new List<TaskColumn>
        {
            new() { Id = 1, UserId = 1, Name = "Backlog",     Color = "var(--tx3)", Position = 0 },
            new() { Id = 2, UserId = 1, Name = "To Do",       Color = "var(--inf)", Position = 1 },
            new() { Id = 3, UserId = 1, Name = "In Progress", Color = "var(--wn)",  Position = 2 },
            new() { Id = 4, UserId = 1, Name = "Done",        Color = "var(--ac)",  Position = 3 },
        };
        db.TaskColumns.AddRange(taskColumns);

        // ── Projects ──────────────────────────────────────────────────────
        var projects = new List<Project>
        {
            new() { Id = 1, UserId = 1, Name = "FERRO",             Description = "Fitness program — strength and conditioning goals.", Status = "active", Color = "#E24B4A", Deadline = new DateOnly(2026, 12, 31), SortOrder = 1 },
            new() { Id = 2, UserId = 1, Name = "Vault App",         Description = "Build the Vault household productivity application.", Status = "active", Color = "#1D9E75", Deadline = new DateOnly(2026, 6, 30),  SortOrder = 2 },
            new() { Id = 3, UserId = 1, Name = "House Reno",        Description = "Home renovation and improvement tasks.",             Status = "active", Color = "#EF9F27", Deadline = null,                        SortOrder = 3 },
            new() { Id = 4, UserId = 1, Name = "Learning ASP.NET",  Description = "Study ASP.NET Core MVC for career development.",     Status = "active", Color = "#7F77DD", Deadline = new DateOnly(2026, 9, 30),  SortOrder = 4 },
        };
        db.Projects.AddRange(projects);

        // ── Task Items ────────────────────────────────────────────────────
        var taskItems = new List<TaskItem>
        {
            new() { Id = 1,  UserId = 1, ColumnId = 1, Title = "Schedule movie night for Hail Mary",         Priority = "low",  Project = "Personal",   ProjectId = null, DueDate = null, Position = 0 },
            new() { Id = 2,  UserId = 1, ColumnId = 1, Title = "Schedule meetup with Carolina",              Priority = "low",  Project = "Personal",   ProjectId = null, DueDate = null, Position = 1 },
            new() { Id = 3,  UserId = 1, ColumnId = 2, Title = "Call pharmacy guy about house",              Priority = "med",  Project = "House Reno", ProjectId = 3,    DueDate = null, Position = 0 },
            new() { Id = 4,  UserId = 1, ColumnId = 2, Title = "Call landlord about leaks",                 Priority = "high", Project = "House Reno", ProjectId = 3,    DueDate = null, Position = 1 },
            new() { Id = 5,  UserId = 1, ColumnId = 2, Title = "Create housing assistance request",         Priority = "med",  Project = "House Reno", ProjectId = 3,    DueDate = null, Position = 2 },
            new() { Id = 6,  UserId = 1, ColumnId = 2, Title = "Municipal assistance for childbirth",       Priority = "high", Project = "House Reno", ProjectId = 3,    DueDate = null, Position = 3 },
            new() { Id = 7,  UserId = 1, ColumnId = 2, Title = "Finance admin",
                    Description = "- Access key for Maura\n- Declaration of no debt\n- Update family aggregate in José\n- Update civil status\n- Redo IRS 2024",
                    Priority = "high", Project = "Finances", ProjectId = null, DueDate = null, Position = 4 },
            new() { Id = 8,  UserId = 1, ColumnId = 2, Title = "Go to Santander — create joint account",   Priority = "med",  Project = "Finances",   ProjectId = null, DueDate = null, Position = 5 },
            new() { Id = 9,  UserId = 1, ColumnId = 2, Title = "Study EF Core migrations",                 Priority = "med",  Project = "Learning ASP.NET", ProjectId = 4, DueDate = new DateOnly(2026, 4, 20), Position = 6 },
            new() { Id = 10, UserId = 1, ColumnId = 3, Title = "Implement Vault backend wiring",           Priority = "high", Project = "Vault App",  ProjectId = 2,    DueDate = new DateOnly(2026, 4, 15), Position = 0 },
        };
        db.TaskItems.AddRange(taskItems);

        // ── Maintenance Items ─────────────────────────────────────────────
        var now = DateTime.UtcNow;
        var maintenanceItems = new List<MaintenanceItem>
        {
            new() { Id = 1, UserId = 1, Name = "Car oil change",      Category = "Vehicle",    Icon = "🚗", IntervalDays = 180, LastCompletedAt = now.AddDays(-120), NextDueAt = now.AddDays(60),   SortOrder = 1 },
            new() { Id = 2, UserId = 1, Name = "Water plants",        Category = "Garden",     Icon = "🌿", IntervalDays = 3,   LastCompletedAt = now.AddDays(-4),   NextDueAt = now.AddDays(-1),   SortOrder = 2 },
            new() { Id = 3, UserId = 1, Name = "Replace toothbrush",  Category = "Health",     Icon = "🪥", IntervalDays = 90,  LastCompletedAt = now.AddDays(-75),  NextDueAt = now.AddDays(15),   SortOrder = 3 },
            new() { Id = 4, UserId = 1, Name = "HVAC filter",         Category = "Home",       Icon = "🏠", IntervalDays = 90,  LastCompletedAt = now.AddDays(-95),  NextDueAt = now.AddDays(-5),   SortOrder = 4 },
            new() { Id = 5, UserId = 1, Name = "Check smoke detectors", Category = "Safety",   Icon = "🔥", IntervalDays = 180, LastCompletedAt = now.AddDays(-30),  NextDueAt = now.AddDays(150),  SortOrder = 5 },
            new() { Id = 6, UserId = 1, Name = "Tire pressure",       Category = "Vehicle",    Icon = "🚗", IntervalDays = 30,  LastCompletedAt = now.AddDays(-10),  NextDueAt = now.AddDays(20),   SortOrder = 6 },
        };
        db.MaintenanceItems.AddRange(maintenanceItems);

        // ── Notes ─────────────────────────────────────────────────────────
        var notes = new List<Note>
        {
            new() { Id = 1, UserId = 1, Title = "FERRO Goals 2026",       Content = "- Bench press 100kg\n- Run 5k under 25min\n- 80kg body weight", IsPinned = true,  IsStarred = true,  Color = "#E24B4A", Tags = "fitness,goals" },
            new() { Id = 2, UserId = 1, Title = "ASP.NET Resources",      Content = "- learn.microsoft.com/aspnet-core\n- Nick Chapsas YouTube\n- Dometrain courses", IsPinned = true, IsStarred = false, Color = "#7F77DD", Tags = "dev,learning" },
            new() { Id = 3, UserId = 1, Title = "House Reno Priorities",  Content = "1. Fix bathroom leak\n2. Paint living room\n3. New kitchen tiles", IsPinned = false, IsStarred = true,  Color = "#EF9F27", Tags = "home" },
            new() { Id = 4, UserId = 1, Title = "Receita Bacalhau",       Content = "Ingredientes: Bacalhau, batatas, alho, azeite, cebola, ovos. Demolhar 24h.", IsPinned = false, IsStarred = false, Color = null, Tags = "receitas" },
            new() { Id = 5, UserId = 1, Title = "Gift Ideas — Ana",       Content = "- Livros de culinária\n- Curso de fotografia\n- Weekend getaway", IsPinned = false, IsStarred = true,  Color = "#C77DD3", Tags = "family" },
            new() { Id = 6, UserId = 1, Title = "Vault Wishlist",         Content = "- Mobile app\n- Voice commands via Nova\n- Export to PDF\n- Dark/Light theme per widget", IsPinned = false, IsStarred = false, Color = "#1D9E75", Tags = "dev,ideas" },
        };
        db.Notes.AddRange(notes);

        // ── Budget Categories ─────────────────────────────────────────────
        var budgetCategories = new List<BudgetCategory>
        {
            new() { Id = 1, UserId = 1, Name = "Alimentação",    MonthlyLimit = 600,  Color = "#1D9E75", Icon = "🛒", SortOrder = 1 },
            new() { Id = 2, UserId = 1, Name = "Transporte",     MonthlyLimit = 300,  Color = "#378ADD", Icon = "🚗", SortOrder = 2 },
            new() { Id = 3, UserId = 1, Name = "Restaurantes",   MonthlyLimit = 300,  Color = "#EF9F27", Icon = "🍽️", SortOrder = 3 },
            new() { Id = 4, UserId = 1, Name = "Serviços",       MonthlyLimit = 200,  Color = "#7F77DD", Icon = "💡", SortOrder = 4 },
            new() { Id = 5, UserId = 1, Name = "Lazer",          MonthlyLimit = 150,  Color = "#C77DD3", Icon = "🎬", SortOrder = 5 },
            new() { Id = 6, UserId = 1, Name = "Saúde",          MonthlyLimit = 200,  Color = "#E24B4A", Icon = "💊", SortOrder = 6 },
            new() { Id = 7, UserId = 1, Name = "Outros",         MonthlyLimit = 250,  Color = "#6e6d68", Icon = "📦", SortOrder = 7 },
        };
        db.BudgetCategories.AddRange(budgetCategories);

        // ── Transactions (this month) ──────────────────────────────────────
        var thisMonth = DateOnly.FromDateTime(DateTime.UtcNow);
        var transactions = new List<Transaction>
        {
            new() { Id = 1, UserId = 1, CategoryId = 1, Amount = 87.50m,  Description = "Mercado semanal",   Payee = "Pingo Doce",    Date = new DateOnly(thisMonth.Year, thisMonth.Month, 3)  },
            new() { Id = 2, UserId = 1, CategoryId = 1, Amount = 43.20m,  Description = "Compras online",    Payee = "Continente",    Date = new DateOnly(thisMonth.Year, thisMonth.Month, 7)  },
            new() { Id = 3, UserId = 1, CategoryId = 2, Amount = 60.00m,  Description = "Combustível",       Payee = "Galp",          Date = new DateOnly(thisMonth.Year, thisMonth.Month, 2)  },
            new() { Id = 4, UserId = 1, CategoryId = 3, Amount = 38.50m,  Description = "Jantar com amigos", Payee = "Restaurante A", Date = new DateOnly(thisMonth.Year, thisMonth.Month, 5)  },
            new() { Id = 5, UserId = 1, CategoryId = 4, Amount = 45.00m,  Description = "Internet + TV",     Payee = "NOS",           Date = new DateOnly(thisMonth.Year, thisMonth.Month, 1),  IsRecurring = true },
            new() { Id = 6, UserId = 1, CategoryId = 6, Amount = 25.00m,  Description = "Farmácia",          Payee = "Farmácia S.",   Date = new DateOnly(thisMonth.Year, thisMonth.Month, 6)  },
        };
        db.Transactions.AddRange(transactions);

        // ── Shopping Lists ─────────────────────────────────────────────────
        var shoppingList = new ShoppingList { Id = 1, UserId = 1, Name = "Compras da Semana", IsDefault = true, Color = "#1D9E75" };
        db.ShoppingLists.Add(shoppingList);

        var shoppingItems = new List<ShoppingItem>
        {
            new() { Id = 1,  ListId = 1, UserId = 1, Name = "Arroz",          Quantity = 2,    Unit = "kg",    Category = "Despensa",   SortOrder = 0 },
            new() { Id = 2,  ListId = 1, UserId = 1, Name = "Massa esparguete", Quantity = 500, Unit = "g",    Category = "Despensa",   SortOrder = 1 },
            new() { Id = 3,  ListId = 1, UserId = 1, Name = "Frango",          Quantity = 1,    Unit = "kg",   Category = "Carnes",     SortOrder = 2 },
            new() { Id = 4,  ListId = 1, UserId = 1, Name = "Ovos",            Quantity = 12,   Unit = "unid", Category = "Frescos",    SortOrder = 3 },
            new() { Id = 5,  ListId = 1, UserId = 1, Name = "Leite",           Quantity = 2,    Unit = "L",    Category = "Laticínios", SortOrder = 4 },
            new() { Id = 6,  ListId = 1, UserId = 1, Name = "Iogurte natural", Quantity = 4,    Unit = "unid", Category = "Laticínios", SortOrder = 5 },
            new() { Id = 7,  ListId = 1, UserId = 1, Name = "Tomates",         Quantity = 500,  Unit = "g",    Category = "Legumes",    SortOrder = 6 },
            new() { Id = 8,  ListId = 1, UserId = 1, Name = "Cebolas",         Quantity = 1,    Unit = "kg",   Category = "Legumes",    SortOrder = 7 },
            new() { Id = 9,  ListId = 1, UserId = 1, Name = "Azeite",          Quantity = 1,    Unit = "L",    Category = "Despensa",   SortOrder = 8,  IsDone = true },
            new() { Id = 10, ListId = 1, UserId = 1, Name = "Detergente roupa", Quantity = 1,   Unit = "unid", Category = "Higiene",    SortOrder = 9  },
            new() { Id = 11, ListId = 1, UserId = 1, Name = "Pasta de dentes", Quantity = 2,    Unit = "unid", Category = "Higiene",    SortOrder = 10 },
        };
        db.ShoppingItems.AddRange(shoppingItems);

        // ── Persons (Birthdays/Anniversaries) ─────────────────────────────
        var persons = new List<Person>
        {
            new() { Id = 1, UserId = 1, Name = "Maura",    Date = new DateOnly(1960, 4, 19), EventType = "birthday",     Category = "family",  ReminderDays = "7,1" },
            new() { Id = 2, UserId = 1, Name = "Mãe",      Date = new DateOnly(1965, 4, 28), EventType = "birthday",     Category = "family",  ReminderDays = "7,1" },
            new() { Id = 3, UserId = 1, Name = "Ricardo",  Date = new DateOnly(1990, 5, 3),  EventType = "birthday",     Category = "friends", ReminderDays = "3,1" },
            new() { Id = 4, UserId = 1, Name = "Pai",      Date = new DateOnly(1958, 6, 15), EventType = "birthday",     Category = "family",  ReminderDays = "7,1" },
            new() { Id = 5, UserId = 1, Name = "Casamento José & Ana", Date = new DateOnly(2023, 7, 2), EventType = "anniversary", Category = "family", ReminderDays = "30,7,1" },
            new() { Id = 6, UserId = 1, Name = "Sofia",    Date = new DateOnly(1995, 9, 10), EventType = "birthday",     Category = "friends", ReminderDays = "3,1" },
        };
        db.Persons.AddRange(persons);

        // ── Chores ────────────────────────────────────────────────────────
        var chores = new List<Chore>
        {
            new() { Id = 1, UserId = 1, Name = "Aspirar sala",          AssignedTo = "José", Frequency = "weekly",  DayOfWeek = 6, EffortPoints = 2, Room = "Sala",        RotationType = "alternating" },
            new() { Id = 2, UserId = 1, Name = "Lavar roupa",           AssignedTo = "Ana",  Frequency = "weekly",  DayOfWeek = 1, EffortPoints = 3, Room = "Lavandaria",  RotationType = "alternating" },
            new() { Id = 3, UserId = 1, Name = "Limpar casa de banho",  AssignedTo = "José", Frequency = "weekly",  DayOfWeek = 6, EffortPoints = 3, Room = "Casa de banho", RotationType = "alternating" },
            new() { Id = 4, UserId = 1, Name = "Cozinhar jantar",       AssignedTo = "Ana",  Frequency = "daily",   DayOfWeek = null, EffortPoints = 2, Room = "Cozinha",   RotationType = "alternating" },
            new() { Id = 5, UserId = 1, Name = "Lavar loiça",           AssignedTo = "José", Frequency = "daily",   DayOfWeek = null, EffortPoints = 1, Room = "Cozinha",   RotationType = "alternating" },
            new() { Id = 6, UserId = 1, Name = "Tirar lixo",            AssignedTo = "José", Frequency = "weekly",  DayOfWeek = 2, EffortPoints = 1, Room = null,          RotationType = "none" },
            new() { Id = 7, UserId = 1, Name = "Regar plantas",         AssignedTo = "Ana",  Frequency = "weekly",  DayOfWeek = 0, EffortPoints = 1, Room = null,          RotationType = "none" },
        };
        db.Chores.AddRange(chores);

        // ── Health Metrics (last 7 days) ───────────────────────────────────
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var healthMetrics = new List<HealthMetric>();
        double[] waterValues  = [1500, 2100, 1800, 2200, 1200, 2000, 1900];
        double[] weightValues = [78.8, 78.6, 78.7, 78.5, 78.4, 78.3, 78.2];
        double[] sleepValues  = [6.5, 7.0, 6.8, 7.2, 6.5, 7.1, 7.0];
        double[] hrValues     = [64, 63, 65, 62, 66, 63, 62];
        for (int i = 6; i >= 0; i--)
        {
            var date = today.AddDays(-i);
            var idx  = 6 - i;
            healthMetrics.Add(new() { UserId = 1, MetricType = "water",  Date = date, Value = waterValues[idx],  Unit = "ml" });
            healthMetrics.Add(new() { UserId = 1, MetricType = "weight", Date = date, Value = weightValues[idx], Unit = "kg" });
            healthMetrics.Add(new() { UserId = 1, MetricType = "sleep",  Date = date, Value = sleepValues[idx],  Unit = "h" });
            healthMetrics.Add(new() { UserId = 1, MetricType = "hr",     Date = date, Value = hrValues[idx],     Unit = "bpm" });
        }
        db.HealthMetrics.AddRange(healthMetrics);

        // ── Pantry Items ───────────────────────────────────────────────────
        var pantryItems = new List<PantryItem>
        {
            new() { Id = 1, UserId = 1, Name = "Arroz",           Category = "Cereais",    Location = "pantry",  Quantity = 1.5m, Unit = "kg",    MinStock = 1,    ExpiryDate = new DateOnly(2027, 1, 1) },
            new() { Id = 2, UserId = 1, Name = "Esparguete",      Category = "Cereais",    Location = "pantry",  Quantity = 0.2m, Unit = "kg",    MinStock = 0.5m                                        },
            new() { Id = 3, UserId = 1, Name = "Atum em lata",    Category = "Conservas",  Location = "pantry",  Quantity = 3,    Unit = "latas", MinStock = 2                                           },
            new() { Id = 4, UserId = 1, Name = "Tomate pelado",   Category = "Conservas",  Location = "pantry",  Quantity = 2,    Unit = "latas"                                                         },
            new() { Id = 5, UserId = 1, Name = "Leite UHT",       Category = "Laticínios", Location = "pantry",  Quantity = 3,    Unit = "L",     MinStock = 2,    ExpiryDate = today.AddDays(5)          },
            new() { Id = 6, UserId = 1, Name = "Iogurte",         Category = "Laticínios", Location = "fridge",  Quantity = 4,    Unit = "unid",  MinStock = 2,    ExpiryDate = today.AddDays(2)          },
            new() { Id = 7, UserId = 1, Name = "Detergente loiça",Category = "Limpeza",    Location = "pantry",  Quantity = 0.3m, Unit = "L",     MinStock = 0.5m                                        },
            new() { Id = 8, UserId = 1, Name = "Pasta de dentes", Category = "Higiene",    Location = "pantry",  Quantity = 1,    Unit = "unid",  MinStock = 1                                           },
        };
        db.PantryItems.AddRange(pantryItems);

        // ── Recipes ───────────────────────────────────────────────────────
        var recipes = new List<Recipe>
        {
            new() { Id = 1, UserId = 1, Name = "Aveia com banana",      Ingredients = "Aveia 60g, leite 250ml, banana 1, mel", Instructions = "Cozinhar aveia no leite, adicionar banana e mel.", PrepMinutes = 5,  CookMinutes = 5,  Servings = 1, Tags = "pequeno-almoço,saudável", Rating = 5, IsFavorite = true  },
            new() { Id = 2, UserId = 1, Name = "Ovos mexidos",          Ingredients = "Ovos 3, manteiga, sal, pimenta",        Instructions = "Bater ovos, cozinhar em lume brando com manteiga.", PrepMinutes = 2,  CookMinutes = 5,  Servings = 1, Tags = "pequeno-almoço,rápido",   Rating = 4, IsFavorite = false },
            new() { Id = 3, UserId = 1, Name = "Frango grelhado",       Ingredients = "Frango 200g, alho, limão, sal, azeite", Instructions = "Marinar frango, grelhar 15min.", PrepMinutes = 10, CookMinutes = 15, Servings = 2, Tags = "almoço,proteína",         Rating = 4, IsFavorite = true  },
            new() { Id = 4, UserId = 1, Name = "Sopa de legumes",       Ingredients = "Cenoura, cebola, batata, abobrinha, alho, azeite", Instructions = "Cozinhar legumes, triturar.", PrepMinutes = 10, CookMinutes = 25, Servings = 4, Tags = "jantar,saudável,sopa",    Rating = 5, IsFavorite = true  },
            new() { Id = 5, UserId = 1, Name = "Massa bolonhesa",       Ingredients = "Massa 200g, carne moída 300g, tomate pelado, cebola, alho, vinho tinto", Instructions = "Refogar carne, adicionar tomate e vinho, cozer 30min. Cozer massa.", PrepMinutes = 10, CookMinutes = 35, Servings = 2, Tags = "jantar,massa",            Rating = 5, IsFavorite = true  },
            new() { Id = 6, UserId = 1, Name = "Salada Caesar",         Ingredients = "Alface, frango, croutons, queijo parmesão, molho Caesar", Instructions = "Misturar tudo, temperar com molho.", PrepMinutes = 10, CookMinutes = 0,  Servings = 2, Tags = "almoço,salada,rápido",    Rating = 4, IsFavorite = false },
        };
        db.Recipes.AddRange(recipes);

        // ── Meal Plans (current week) ──────────────────────────────────────
        var monday = today.AddDays(-(int)today.DayOfWeek + (today.DayOfWeek == DayOfWeek.Sunday ? -6 : 1));
        var mealPlans = new List<MealPlan>
        {
            new() { UserId = 1, Date = monday,          Slot = "breakfast", RecipeId = 1 },
            new() { UserId = 1, Date = monday,          Slot = "lunch",     RecipeId = 3 },
            new() { UserId = 1, Date = monday,          Slot = "dinner",    RecipeId = 5 },
            new() { UserId = 1, Date = monday.AddDays(1), Slot = "breakfast", RecipeId = 2 },
            new() { UserId = 1, Date = monday.AddDays(1), Slot = "lunch",   RecipeId = 6 },
            new() { UserId = 1, Date = monday.AddDays(1), Slot = "dinner",  RecipeId = 4 },
            new() { UserId = 1, Date = monday.AddDays(2), Slot = "breakfast", RecipeId = 1 },
        };
        db.MealPlans.AddRange(mealPlans);

        // ── Utility Meters ────────────────────────────────────────────────
        var meters = new List<Meter>
        {
            new() { Id = 1, UserId = 1, Name = "Electricidade", MeterType = "electricity", Unit = "kWh", TariffRate = 0.22m  },
            new() { Id = 2, UserId = 1, Name = "Gás",           MeterType = "gas",          Unit = "m³",  TariffRate = 1.15m  },
            new() { Id = 3, UserId = 1, Name = "Água",          MeterType = "water",        Unit = "m³",  TariffRate = 1.80m  },
        };
        db.Meters.AddRange(meters);

        // ── Meter Readings (6 months) ─────────────────────────────────────
        var meterReadings = new List<MeterReading>();
        // Electricity readings: ~300 kWh/month
        decimal[] elecReadings = [12000, 12300, 12590, 12895, 13185, 13480, 13760];
        // Gas readings: ~15 m³/month
        decimal[] gasReadings  = [500, 515, 529, 543, 558, 572, 587];
        // Water readings: ~10 m³/month
        decimal[] waterReadings = [200, 210, 219, 229, 239, 249, 259];

        for (int m = 0; m <= 6; m++)
        {
            var readingDate = today.AddMonths(-6 + m).AddDays(1 - today.Day); // 1st of each month
            meterReadings.Add(new() { MeterId = 1, UserId = 1, ReadingDate = readingDate, Value = elecReadings[m]  });
            meterReadings.Add(new() { MeterId = 2, UserId = 1, ReadingDate = readingDate, Value = gasReadings[m]   });
            meterReadings.Add(new() { MeterId = 3, UserId = 1, ReadingDate = readingDate, Value = waterReadings[m] });
        }
        db.MeterReadings.AddRange(meterReadings);

        // ── XP Events ─────────────────────────────────────────────────────
        var xpEvents = new List<XpEvent>
        {
            new() { UserId = 1, Action = "habit_done", Points = 10, LinkedModule = "habit", LinkedId = 3, CreatedAt = DateTime.UtcNow.AddDays(-1) },
            new() { UserId = 1, Action = "habit_done", Points = 10, LinkedModule = "habit", LinkedId = 2, CreatedAt = DateTime.UtcNow.AddDays(-1) },
            new() { UserId = 1, Action = "task_done",  Points = 25, LinkedModule = "task",  LinkedId = 5, CreatedAt = DateTime.UtcNow.AddDays(-2) },
            new() { UserId = 2, Action = "habit_done", Points = 10, LinkedModule = "habit", LinkedId = 3, CreatedAt = DateTime.UtcNow.AddDays(-1) },
            new() { UserId = 2, Action = "chore_done", Points = 30, LinkedModule = "chore", LinkedId = 2, CreatedAt = DateTime.UtcNow.AddDays(-2) },
        };
        db.XpEvents.AddRange(xpEvents);

        // ── Templates ─────────────────────────────────────────────────────
        var templates = new List<Template>
        {
            new() { Id = 1,  Name = "Morning Routine",      Description = "Daily morning habits: water, exercise, meditation",  Category = "habits",      Icon = "🌅", IsBuiltIn = true, Payload = """{"habits":[{"name":"Drink Water","type":"boolean","freq":"Daily"},{"name":"Exercise","type":"boolean","freq":"Daily"},{"name":"Meditate","type":"boolean","freq":"Daily"}]}""" },
            new() { Id = 2,  Name = "Weekly Review",        Description = "End-of-week reflection tasks",                       Category = "tasks",       Icon = "📋", IsBuiltIn = true, Payload = """{"tasks":[{"title":"Review last week goals","priority":"med"},{"title":"Plan next week","priority":"high"},{"title":"Update progress tracker","priority":"low"}]}""" },
            new() { Id = 3,  Name = "Home Deep Clean",      Description = "Thorough home cleaning checklist",                   Category = "tasks",       Icon = "🏠", IsBuiltIn = true, Payload = """{"tasks":[{"title":"Clean kitchen thoroughly","priority":"med"},{"title":"Clean bathrooms","priority":"med"},{"title":"Vacuum all rooms","priority":"low"},{"title":"Mop floors","priority":"low"}]}""" },
            new() { Id = 4,  Name = "Fitness Program",      Description = "Weekly workout habit setup",                         Category = "habits",      Icon = "💪", IsBuiltIn = true, Payload = """{"habits":[{"name":"Workout","type":"boolean","freq":"Daily","weeklyGoal":5},{"name":"Walk 8k steps","type":"quant","freq":"Daily","goal":8000,"uom":"steps"}]}""" },
            new() { Id = 5,  Name = "Side Project Launch",  Description = "Project setup tasks for a new side project",         Category = "projects",    Icon = "🚀", IsBuiltIn = true, Payload = """{"project":{"name":"New Project","status":"active"},"tasks":[{"title":"Define MVP scope","priority":"high"},{"title":"Set up repository","priority":"high"},{"title":"Create roadmap","priority":"med"}]}""" },
            new() { Id = 6,  Name = "Grocery Run",          Description = "Standard weekly grocery list",                       Category = "tasks",       Icon = "🛒", IsBuiltIn = true, Payload = """{"tasks":[{"title":"Buy groceries","priority":"med"}]}""" },
            new() { Id = 7,  Name = "Seasonal Maintenance", Description = "Quarterly home maintenance checklist",               Category = "maintenance", Icon = "🔧", IsBuiltIn = true, Payload = """{"items":[{"name":"Check HVAC filter","intervalDays":90},{"name":"Clean gutters","intervalDays":180},{"name":"Test smoke detectors","intervalDays":180}]}""" },
            new() { Id = 8,  Name = "Reading Habit",        Description = "Daily reading habit setup",                          Category = "habits",      Icon = "📚", IsBuiltIn = true, Payload = """{"habits":[{"name":"Read 30min","type":"quant","freq":"Daily","goal":30,"uom":"min","weeklyGoal":7}]}""" },
            new() { Id = 9,  Name = "Budget Review",        Description = "Monthly budget review tasks",                        Category = "tasks",       Icon = "💰", IsBuiltIn = true, Payload = """{"tasks":[{"title":"Review monthly spending","priority":"high"},{"title":"Categorize transactions","priority":"med"},{"title":"Adjust budget limits","priority":"low"}]}""" },
            new() { Id = 10, Name = "Travel Prep",          Description = "Pre-travel checklist tasks",                         Category = "tasks",       Icon = "✈️", IsBuiltIn = true, Payload = """{"tasks":[{"title":"Book accommodation","priority":"high"},{"title":"Pack bags","priority":"med"},{"title":"Arrange pet care","priority":"med"},{"title":"Notify bank","priority":"low"}]}""" },
            new() { Id = 11, Name = "Language Learning",    Description = "Daily language practice habit",                      Category = "habits",      Icon = "🌍", IsBuiltIn = true, Payload = """{"habits":[{"name":"Language practice","type":"quant","freq":"Daily","goal":15,"uom":"min","weeklyGoal":7}]}""" },
            new() { Id = 12, Name = "Car Maintenance",      Description = "Regular car maintenance schedule",                   Category = "maintenance", Icon = "🚗", IsBuiltIn = true, Payload = """{"items":[{"name":"Oil change","intervalDays":180},{"name":"Tire rotation","intervalDays":90},{"name":"Tire pressure check","intervalDays":30}]}""" },
        };
        db.Templates.AddRange(templates);

        await db.SaveChangesAsync();
    }
}
