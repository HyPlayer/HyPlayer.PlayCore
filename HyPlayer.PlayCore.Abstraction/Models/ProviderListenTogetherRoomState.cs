namespace HyPlayer.PlayCore.Abstraction.Models;

public sealed class ProviderListenTogetherRoomState
{
    public List<string> DisplayItemIds { get; init; } = [];

    public List<string> RandomItemIds { get; init; } = [];

    public string? PlayModeId { get; init; }

    public List<ProviderListenTogetherQueueVersion> Versions { get; init; } = [];

    public ProviderListenTogetherRemoteCommand? LastCommand { get; init; }
}

public sealed class ProviderListenTogetherQueueVersion
{
    public required string UserId { get; init; }

    public long Version { get; init; }
}

public sealed class ProviderListenTogetherRemoteCommand
{
    public required string CommandId { get; init; }

    public string? UserId { get; init; }

    public string? FormerItemId { get; init; }

    public string? TargetItemId { get; init; }

    public TimeSpan Position { get; init; }

    public bool IsPlaying { get; init; }

    public long ClientSeq { get; init; }

    public long ServerSeq { get; init; }
}
