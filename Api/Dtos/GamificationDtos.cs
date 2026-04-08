namespace Vault.Api.Dtos;

public sealed record UserLeaderboardEntry(
    int UserId,
    string Name,
    string? AvatarColor,
    int TotalXp,
    int Level,
    string LevelTitle,
    int XpForCurrentLevel,
    int XpForNextLevel,
    int XpIntoCurrentLevel,
    int XpPct,
    int PeriodXp,
    int HabitsDone,
    int TasksDone,
    int ChoresDone
);

public sealed record LeaderboardResponse(
    string Period,
    IReadOnlyList<UserLeaderboardEntry> Users
);

public sealed record XpEventResponse(
    int Id,
    string Action,
    int Points,
    string? LinkedModule,
    int? LinkedId,
    string CreatedAt
);

public sealed record ChallengeResponse(
    int Id,
    string Title,
    string? Description,
    string TargetModule,
    int TargetCount,
    string StartDate,
    string EndDate,
    IReadOnlyList<ChallengeProgressResponse> Progress
);

public sealed record ChallengeProgressResponse(
    int UserId,
    string UserName,
    int CurrentCount,
    int TargetCount,
    int Pct
);

public sealed record CreateChallengeRequest(
    string Title,
    string? Description,
    string TargetModule,
    int TargetCount,
    string StartDate,
    string EndDate
);
