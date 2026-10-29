namespace HyPlayer.PlayCore.Abstraction.Models;

public sealed class ProviderMultiListenTogetherRoom
{
    public required string RoomId { get; init; }
    public string? CreatorId { get; init; }
    public List<ProviderListenTogetherUser> Users { get; init; } = [];
    public ProviderMultiListenTogetherPlayback? Playback { get; init; }
    public bool CanRecommend { get; init; } = true;
    public bool CanRequestSkip { get; init; } = true;
}

public sealed class ProviderMultiListenTogetherPlayback
{
    public required string ItemId { get; init; }
    public required string EntryId { get; init; }
    public string? RecommenderId { get; init; }
    public TimeSpan Position { get; init; }
    public TimeSpan Duration { get; init; }
    public long Version { get; init; }
    public List<string> NextItemIds { get; init; } = [];
}

public sealed class ProviderMultiListenTogetherHeartbeat
{
    public ProviderMultiListenTogetherPlayback? Playback { get; init; }
    public TimeSpan? Interval { get; init; }
    public bool IsClosed { get; init; }
}

public sealed class ProviderMultiListenTogetherQueueItem
{
    public required string ItemId { get; init; }
    public required string EntryId { get; init; }
    public required string Title { get; init; }
    public string Artists { get; init; } = string.Empty;
    public string? CoverUrl { get; init; }
    public string? RecommenderId { get; init; }
    public string RecommenderName { get; init; } = string.Empty;
    public bool IsSystemRecommendation { get; init; }
    public bool IsPlaying { get; init; }
}

public sealed class ProviderMultiListenTogetherOperation
{
    public bool Succeeded { get; init; }
    public string Message { get; init; } = string.Empty;
    public ProviderMultiListenTogetherPlayback? Playback { get; init; }
}
