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
    private readonly SemaphoreSlim _settlementGate = new(1, 1);
    private PreparedPlaybackTicket? _outgoing;

    public PreparedPlaybackPromotion(
        PreparedPlaybackTicket incoming,
        PreparedPlaybackTicket? outgoing)
    {
        Incoming = incoming ?? throw new ArgumentNullException(nameof(incoming));
        _outgoing = outgoing;
    }

    public PreparedPlaybackTicket Incoming { get; }

    public PreparedPlaybackTicket? Outgoing => Volatile.Read(ref _outgoing);

    /// <summary>
    /// Permanently settles the outgoing ticket. Once settlement starts it is deliberately
    /// non-cancellable so a cancelled playback request cannot leave an audible orphan.
    /// </summary>
    public async Task SettleOutgoingAsync()
    {
        await _settlementGate.WaitAsync().ConfigureAwait(false);
        try
        {
            var outgoing = _outgoing;
            if (outgoing is null)
                return;

            try
            {
                await outgoing.SetVolumeAsync(0, CancellationToken.None).ConfigureAwait(false);
            }
            catch
            {
                // Disposal below is the authoritative silence/detach boundary.
            }

            try
            {
                await outgoing.PauseAsync(CancellationToken.None).ConfigureAwait(false);
            }
            catch
            {
                // Disposal below is the authoritative silence/detach boundary.
            }

            await outgoing.DisposeAsync().ConfigureAwait(false);
            Interlocked.CompareExchange(ref _outgoing, null, outgoing);
        }
        finally
        {
            _settlementGate.Release();
        }
    }
}
