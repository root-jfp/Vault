using Vault.Api.Dtos;
using Vault.Services;

namespace Vault.Api;

public static class GamificationEndpoints
{
    public static WebApplication MapGamificationEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/gamification");

        group.MapGet("/leaderboard", async (string? period, GamificationService svc) =>
            Results.Ok(await svc.GetLeaderboardAsync(period ?? "week")));

        group.MapGet("/xp-history", async (int? days, GamificationService svc) =>
            Results.Ok(await svc.GetXpHistoryAsync(days ?? 30)));

        group.MapGet("/challenges", async (GamificationService svc) =>
            Results.Ok(await svc.GetChallengesAsync()));

        group.MapPost("/challenges", async (CreateChallengeRequest req, GamificationService svc) =>
        {
            if (string.IsNullOrWhiteSpace(req.Title))
                return Results.BadRequest(new { error = "Title is required" });
            var created = await svc.CreateChallengeAsync(req);
            return Results.Created($"/api/gamification/challenges/{created.Id}", created);
        });

        group.MapDelete("/challenges/{id:int}", async (int id, GamificationService svc) =>
        {
            var deleted = await svc.DeleteChallengeAsync(id);
            return deleted ? Results.Ok(new { success = true }) : Results.NotFound(new { error = "Challenge not found" });
        });

        return app;
    }
}
