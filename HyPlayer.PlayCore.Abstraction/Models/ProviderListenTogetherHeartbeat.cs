namespace HyPlayer.PlayCore.Abstraction.Models;

public sealed class ProviderListenTogetherHeartbeat
{
    public required string ItemId { get; init; }

    public bool IsPlaying { get; init; }

    public TimeSpan Position { get; init; }

    public string? UserId { get; init; }

    public long PlaylistVersion { get; init; }
}
