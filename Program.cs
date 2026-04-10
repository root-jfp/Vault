using Microsoft.EntityFrameworkCore;
using Vault.Api;
using Vault.Data;
using Vault.Services;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = ".",
});

var dbPath = Environment.GetEnvironmentVariable("VAULT_DB_PATH")
    ?? Path.Combine(AppContext.BaseDirectory, "vault.db");

builder.Services.AddDbContext<VaultDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

builder.WebHost.ConfigureKestrel(options =>
    options.Limits.MaxRequestBodySize = 64 * 1024);

// Services — Sprint 0 (foundation)
builder.Services.AddScoped<UserService>();
// Sprint existing
builder.Services.AddScoped<HabitService>();
builder.Services.AddScoped<TaskService>();
// Sprint 1-16
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<MaintenanceService>();
builder.Services.AddScoped<NoteService>();
builder.Services.AddScoped<BudgetService>();
builder.Services.AddScoped<ShoppingService>();
builder.Services.AddScoped<BirthdayService>();
builder.Services.AddScoped<ChoreService>();
builder.Services.AddScoped<HealthService>();
builder.Services.AddScoped<PantryService>();
builder.Services.AddScoped<MealService>();
builder.Services.AddScoped<MeterService>();
builder.Services.AddScoped<CalendarService>();
builder.Services.AddScoped<PerformanceService>();
builder.Services.AddScoped<GamificationService>();
builder.Services.AddScoped<TemplateService>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy =
        System.Text.Json.JsonNamingPolicy.CamelCase;
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append("Content-Security-Policy",
        "default-src 'self'; script-src 'self' https://cdn.tailwindcss.com https://unpkg.com https://cdn.jsdelivr.net 'unsafe-inline' 'unsafe-eval'; style-src 'self' 'unsafe-inline'; img-src 'self' data: blob:; connect-src 'self' https://cdn.tailwindcss.com https://api.open-meteo.com;");
    await next();
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<VaultDbContext>();
    await SeedData.InitializeAsync(db);
}

app.UseDefaultFiles(new DefaultFilesOptions
{
    DefaultFileNames = ["index.html"]
});

app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = false,
    OnPrepareResponse = ctx =>
    {
        var ext = Path.GetExtension(ctx.File.Name);
        if (ext is ".html")
            ctx.Context.Response.Headers.CacheControl = "no-cache";
    }
});

// Map all API endpoints
app.MapUserEndpoints();
app.MapHabitEndpoints();
app.MapTaskEndpoints();
app.MapProjectEndpoints();
app.MapMaintenanceEndpoints();
app.MapNoteEndpoints();
app.MapBudgetEndpoints();
app.MapShoppingEndpoints();
app.MapBirthdayEndpoints();
app.MapChoreEndpoints();
app.MapHealthEndpoints();
app.MapPantryEndpoints();
app.MapMealEndpoints();
app.MapMeterEndpoints();
app.MapCalendarEndpoints();
app.MapPerformanceEndpoints();
app.MapGamificationEndpoints();
app.MapTemplateEndpoints();

app.MapGet("/", () => Results.Redirect("/index.html"));

app.Run();
