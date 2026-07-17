using HyPlayer.PlayCore.Abstraction.Models.SingleItems;

namespace HyPlayer.PlayCore.Abstraction.Models.AudioServiceComponents;

/// <summary>An owned playback ticket prepared for a possible transition.</summary>
public abstract class PreparedPlaybackTicket
{
    public abstract SingleSongBase Song { get; }
    public abstract double TargetVolume { get; }
    public abstract Task PlayAsync(CancellationToken ctk = default);
    public abstract Task PauseAsync(CancellationToken ctk = default);
    public abstract Task SetVolumeAsync(double volume, CancellationToken ctk = default);
    public abstract Task SetPlaybackRateAsync(double playbackRate, CancellationToken ctk = default);
    public abstract Task DisposeAsync();
}

public sealed class PreparedPlaybackPromotion
{
    public required PreparedPlaybackTicket Incoming { get; init; }
    public PreparedPlaybackTicket? Outgoing { get; init; }
}
