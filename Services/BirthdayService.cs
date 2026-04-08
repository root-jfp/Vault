using Microsoft.EntityFrameworkCore;
using Vault.Api.Dtos;
using Vault.Data;
using Vault.Data.Models;

namespace Vault.Services;

public sealed class BirthdayService(VaultDbContext db)
{
    private const int DefaultUserId = 1;

    public async Task<IReadOnlyList<PersonResponse>> GetAllAsync()
    {
        var persons = await db.Persons
            .Where(p => p.UserId == DefaultUserId)
            .OrderBy(p => p.Name)
            .AsNoTracking()
            .ToListAsync();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return persons.Select(p => MapResponse(p, today)).OrderBy(p => p.DaysUntil).ToList();
    }

    public async Task<PersonResponse?> GetByIdAsync(int id)
    {
        var p = await db.Persons.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id && p.UserId == DefaultUserId);
        return p is null ? null : MapResponse(p, DateOnly.FromDateTime(DateTime.UtcNow));
    }

    public async Task<IReadOnlyList<PersonResponse>> GetUpcomingAsync(int days = 30)
    {
        var all = await GetAllAsync();
        return all.Where(p => p.DaysUntil >= 0 && p.DaysUntil <= days).ToList();
    }

    public async Task<PersonResponse> CreateAsync(CreatePersonRequest req)
    {
        if (!DateOnly.TryParseExact(req.Date, "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var date))
            date = DateOnly.FromDateTime(DateTime.UtcNow);
        var person = new Person
        {
            UserId      = DefaultUserId,
            Name        = req.Name,
            Date        = date,
            EventType   = NormalizeType(req.EventType),
            Category    = req.Category,
            Notes       = req.Notes,
            ReminderDays = req.ReminderDays,
        };
        db.Persons.Add(person);
        await db.SaveChangesAsync();
        return MapResponse(person, DateOnly.FromDateTime(DateTime.UtcNow));
    }

    public async Task<PersonResponse?> UpdateAsync(int id, UpdatePersonRequest req)
    {
        var p = await db.Persons.FirstOrDefaultAsync(p => p.Id == id && p.UserId == DefaultUserId);
        if (p is null) return null;
        if (req.Name         is not null) p.Name         = req.Name;
        if (req.EventType    is not null) p.EventType    = NormalizeType(req.EventType);
        if (req.Category     is not null) p.Category     = req.Category;
        if (req.Notes        is not null) p.Notes        = req.Notes;
        if (req.ReminderDays is not null) p.ReminderDays = req.ReminderDays;
        if (req.Date is not null && DateOnly.TryParseExact(req.Date, "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var date))
            p.Date = date;
        p.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return MapResponse(p, DateOnly.FromDateTime(DateTime.UtcNow));
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var p = await db.Persons.FirstOrDefaultAsync(p => p.Id == id && p.UserId == DefaultUserId);
        if (p is null) return false;
        p.DeletedAt = DateTime.UtcNow;
        p.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return true;
    }

    private static PersonResponse MapResponse(Person p, DateOnly today)
    {
        var nextOccurrence = new DateOnly(today.Year, p.Date.Month, p.Date.Day);
        if (nextOccurrence < today) nextOccurrence = nextOccurrence.AddYears(1);
        var daysUntil = nextOccurrence.DayNumber - today.DayNumber;

        int? nextAge = p.EventType == "birthday" ? today.Year - p.Date.Year + (nextOccurrence.Year > today.Year ? 1 : 0) : null;
        int? yearsTogether = p.EventType == "anniversary" ? today.Year - p.Date.Year + (nextOccurrence.Year > today.Year ? 1 : 0) : null;

        var milestoneAges        = new HashSet<int> { 18, 21, 25, 30, 40, 50, 60, 70, 80, 90, 100 };
        var milestoneAnniversary = new HashSet<int> { 1, 5, 10, 15, 20, 25, 30, 40, 50 };
        bool isMilestone = (nextAge.HasValue && milestoneAges.Contains(nextAge.Value))
                        || (yearsTogether.HasValue && milestoneAnniversary.Contains(yearsTogether.Value));

        return new PersonResponse(
            p.Id, p.Name, p.Date.ToString("yyyy-MM-dd"), p.EventType,
            p.Category, p.Notes, p.ReminderDays,
            daysUntil, nextAge, yearsTogether, isMilestone);
    }

    private static string NormalizeType(string? s) =>
        s switch { "anniversary" => "anniversary", "other" => "other", _ => "birthday" };
}
