using Microsoft.EntityFrameworkCore;
using Vault.Api.Dtos;
using Vault.Data;
using Vault.Data.Models;

namespace Vault.Services;

public sealed class MeterService(VaultDbContext db)
{
    private const int DefaultUserId = 1;

    public async Task<IReadOnlyList<MeterResponse>> GetAllAsync()
    {
        var meters = await db.Meters
            .Where(m => m.UserId == DefaultUserId)
            .Include(m => m.Readings.OrderByDescending(r => r.ReadingDate).Take(3))
            .OrderBy(m => m.Id)
            .AsNoTracking()
            .ToListAsync();
        return meters.Select(MapResponse).ToList();
    }

    public async Task<(MeterResponse meter, IReadOnlyList<MeterReadingResponse> readings)?> GetByIdAsync(int id, int months = 6)
    {
        var meter = await db.Meters
            .Include(m => m.Readings.OrderByDescending(r => r.ReadingDate).Take(months + 1))
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == DefaultUserId);
        if (meter is null) return null;

        var readings = meter.Readings.OrderBy(r => r.ReadingDate).ToList();
        var readingResponses = new List<MeterReadingResponse>();
        for (int i = 0; i < readings.Count; i++)
        {
            decimal? usage = i > 0 ? readings[i].Value - readings[i - 1].Value : null;
            readingResponses.Add(new MeterReadingResponse(
                readings[i].Id, readings[i].ReadingDate.ToString("yyyy-MM-dd"),
                readings[i].Value, usage, readings[i].Note));
        }

        return (MapResponse(meter), readingResponses);
    }

    public async Task<MeterResponse> CreateAsync(CreateMeterRequest req)
    {
        var meter = new Meter
        {
            UserId     = DefaultUserId,
            Name       = req.Name,
            MeterType  = req.MeterType ?? "electricity",
            Unit       = req.Unit ?? "kWh",
            TariffRate = req.TariffRate ?? 0,
        };
        db.Meters.Add(meter);
        await db.SaveChangesAsync();
        return MapResponse(meter);
    }

    public async Task<MeterResponse?> UpdateAsync(int id, UpdateMeterRequest req)
    {
        var m = await db.Meters.FirstOrDefaultAsync(m => m.Id == id && m.UserId == DefaultUserId);
        if (m is null) return null;
        if (req.Name       is not null) m.Name       = req.Name;
        if (req.TariffRate is not null) m.TariffRate = req.TariffRate.Value;
        m.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return MapResponse(m);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var m = await db.Meters.FirstOrDefaultAsync(m => m.Id == id && m.UserId == DefaultUserId);
        if (m is null) return false;
        m.DeletedAt = DateTime.UtcNow;
        m.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<MeterReadingResponse?> LogReadingAsync(int meterId, LogReadingRequest req)
    {
        var meterExists = await db.Meters.AnyAsync(m => m.Id == meterId && m.UserId == DefaultUserId);
        if (!meterExists) return null;

        var date = ParseDate(req.ReadingDate) ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var prevReading = await db.MeterReadings
            .Where(r => r.MeterId == meterId && r.ReadingDate < date)
            .OrderByDescending(r => r.ReadingDate)
            .FirstOrDefaultAsync();

        var reading = new MeterReading
        {
            MeterId     = meterId,
            UserId      = DefaultUserId,
            ReadingDate = date,
            Value       = req.Value,
            Note        = req.Note,
        };
        db.MeterReadings.Add(reading);

        // Update meter's current reading
        var meter = await db.Meters.FindAsync(meterId);
        if (meter is not null)
        {
            meter.CurrentReading = req.Value;
            meter.UpdatedAt      = DateTime.UtcNow;
        }

        await db.SaveChangesAsync();
        decimal? usage = prevReading is not null ? req.Value - prevReading.Value : null;
        return new MeterReadingResponse(reading.Id, date.ToString("yyyy-MM-dd"), reading.Value, usage, reading.Note);
    }

    private static MeterResponse MapResponse(Meter m)
    {
        var readings = m.Readings.OrderByDescending(r => r.ReadingDate).ToList();
        decimal? latest      = readings.Count > 0 ? readings[0].Value : null;
        string? latestDate   = readings.Count > 0 ? readings[0].ReadingDate.ToString("yyyy-MM-dd") : null;
        decimal? monthlyUsage = readings.Count >= 2 ? readings[0].Value - readings[1].Value : null;
        decimal? vsLastMonth = null;
        if (readings.Count >= 3)
        {
            var prevUsage = readings[1].Value - readings[2].Value;
            if (prevUsage != 0) vsLastMonth = Math.Round((monthlyUsage!.Value - prevUsage) / prevUsage * 100, 1);
        }
        decimal? estimatedBill = monthlyUsage.HasValue ? Math.Round(monthlyUsage.Value * m.TariffRate, 2) : null;
        return new MeterResponse(m.Id, m.Name, m.MeterType, m.Unit, m.TariffRate,
            latest, latestDate, monthlyUsage, vsLastMonth, estimatedBill);
    }

    private static DateOnly? ParseDate(string? s) =>
        DateOnly.TryParseExact(s, "yyyy-MM-dd",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out var d) ? d : null;
}
