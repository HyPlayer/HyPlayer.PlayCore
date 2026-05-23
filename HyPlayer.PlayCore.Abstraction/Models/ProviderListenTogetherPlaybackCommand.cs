namespace HyPlayer.PlayCore.Abstraction.Models;

public sealed class ProviderListenTogetherPlaybackCommand
{
    public required string CommandId { get; init; }
    public TimeSpan Position { get; init; }
    public bool IsPlaying { get; init; }
    public string? FormerItemId { get; init; }
    public string? TargetItemId { get; init; }
    public int ClientSeq { get; init; }
}
