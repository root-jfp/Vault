using Microsoft.EntityFrameworkCore;
using Vault.Api.Dtos;
using Vault.Data;
using Vault.Data.Models;

namespace Vault.Services;

public sealed class UserService(VaultDbContext db)
{
    public async Task<IReadOnlyList<UserResponse>> GetAllAsync()
    {
        var users = await db.UserProfiles.AsNoTracking().OrderBy(u => u.Id).ToListAsync();
        return users.Select(Map).ToList();
    }

    public async Task<UserResponse?> GetByIdAsync(int id)
    {
        var user = await db.UserProfiles.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        return user is null ? null : Map(user);
    }

    public async Task<UserResponse> CreateAsync(CreateUserRequest req)
    {
        var user = new UserProfile { Name = req.Name, AvatarColor = req.AvatarColor };
        db.UserProfiles.Add(user);
        await db.SaveChangesAsync();
        return Map(user);
    }

    public async Task<UserResponse?> UpdateAsync(int id, UpdateUserRequest req)
    {
        var user = await db.UserProfiles.FirstOrDefaultAsync(u => u.Id == id);
        if (user is null) return null;

        // UserProfile uses init-only — create replacement via EF tracking
        db.Entry(user).CurrentValues.SetValues(new
        {
            user.Id,
            Name = req.Name ?? user.Name,
            AvatarColor = req.AvatarColor ?? user.AvatarColor,
            user.CreatedAt,
            UpdatedAt = DateTime.UtcNow,
        });
        await db.SaveChangesAsync();
        return (await GetByIdAsync(id))!;
    }

    private static UserResponse Map(UserProfile u) => new(u.Id, u.Name, u.AvatarColor);
}
