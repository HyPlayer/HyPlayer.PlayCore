using Depository.Abstraction.Interfaces;
using HyPlayer.PlayCore.Abstraction.Interfaces.AudioServices;
using HyPlayer.PlayCore.Abstraction.Interfaces.Provider;
using HyPlayer.PlayCore.Abstraction;
using HyPlayer.PlayCore.Abstraction.Models.AudioServiceComponents;
using HyPlayer.PlayCore.Abstraction.Models.Notifications;
using HyPlayer.PlayCore.Abstraction.Models.Resources;
using HyPlayer.PlayCore.Abstraction.Models.SingleItems;

namespace HyPlayer.PlayCore;

public sealed partial class Chopin
{
    public override AudioTicketBase? CurrentPlayingTicket { get; protected set; }

    private async Task<AudioTicketBase?> EnsureCurrentPlayingTicketAsync(CancellationToken ctk)
    {
        var requestedSong = CurrentSong;
        if (requestedSong is null)
        {
            await PublishPlaybackFailureAsync(null, "No current song is selected.", null, ctk).ConfigureAwait(false);
            await DisposeCurrentPlayingTicketAsync(false, ctk).ConfigureAwait(false);
            return null;
        }

        var audioService = CurrentAudioService;
        if (audioService is null)
        {
            await PublishPlaybackFailureAsync(requestedSong, "No current audio service is selected.", null, ctk).ConfigureAwait(false);
            await DisposeCurrentPlayingTicketAsync(false, ctk).ConfigureAwait(false);
            return null;
        }

        if (CurrentPlayingTicket?.AudioServiceId == audioService.Id
            && ReferenceEquals(LastRequestedPlaybackSong, requestedSong))
            return CurrentPlayingTicket;

        await DisposeCurrentPlayingTicketAsync(false, ctk).ConfigureAwait(false);
        ctk.ThrowIfCancellationRequested();

        var resource = await ResolveMusicResourceAsync(requestedSong, ctk).ConfigureAwait(false);
        if (resource is null)
        {
            await PublishPlaybackFailureAsync(requestedSong, "No matching music resource provider returned a resource.", null, ctk).ConfigureAwait(false);
            return null;
        }

        if (!ReferenceEquals(CurrentSong, requestedSong) || !ReferenceEquals(CurrentAudioService, audioService))
            return null;

        var ticket = await audioService.GetAudioTicketAsync(resource, ctk).ConfigureAwait(false);
        if (ctk.IsCancellationRequested
            || !ReferenceEquals(CurrentSong, requestedSong)
            || !ReferenceEquals(CurrentAudioService, audioService))
        {
            await audioService.DisposeAudioTicketAsync(ticket, CancellationToken.None).ConfigureAwait(false);
            ctk.ThrowIfCancellationRequested();
            return null;
        }

        CurrentPlayingTicket = ticket;
        LastRequestedPlaybackSong = requestedSong;
        return CurrentPlayingTicket;
    }

    private AudioServiceBase? FindAudioServiceForTicket(AudioTicketBase ticket)
    {
        if (CurrentAudioService?.Id == ticket.AudioServiceId)
            return CurrentAudioService;

        return AudioServices?.FirstOrDefault(service => service.Id == ticket.AudioServiceId);
    }

    private async Task DisposeCurrentPlayingTicketAsync(bool stopTicket, CancellationToken ctk)
    {
        ctk.ThrowIfCancellationRequested();
        if (CurrentPlayingTicket is not { } ticket)
            return;

        var audioService = FindAudioServiceForTicket(ticket)
            ?? throw new InvalidOperationException($"The audio service for ticket '{ticket.AudioServiceId}' is unavailable.");

        Exception? stopException = null;
        if (stopTicket && audioService is IStopAudioTicketService stopService)
        {
            try
            {
                await stopService.StopTicketAsync(ticket, CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                stopException = ex;
            }
        }

        await audioService.DisposeAudioTicketAsync(ticket, CancellationToken.None).ConfigureAwait(false);
        if (ReferenceEquals(CurrentPlayingTicket, ticket))
            ClearCurrentPlayingTicket();

        if (stopException is not null)
            throw stopException;

        ctk.ThrowIfCancellationRequested();
    }

    private async Task<MusicResourceBase?> ResolveMusicResourceAsync(SingleSongBase song, CancellationToken ctk)
    {
        var provider = MusicProviders?
            .OfType<IMusicResourceProvidable>()
            .FirstOrDefault(p => p.Id == song.ProviderId);

        if (provider is null)
            return null;

        return await provider.GetMusicResourceAsync(song, null, ctk).ConfigureAwait(false);
    }

    private Task PublishPlaybackFailureAsync(SingleSongBase? song, string reason, Exception? exception, CancellationToken ctk)
    {
        if (ResolveOptional<INotificationHub>(typeof(INotificationHub)) is not { } notificationHub)
            return Task.CompletedTask;

        return notificationHub.PublishNotificationAsync(new PlaybackRequestFailedNotification
        {
            Song = song,
            Reason = reason,
            Exception = exception
        }, ctk);
    }

    public override async Task<PreparedPlaybackTicket?> PreparePlaybackAsync(
        SingleSongBase song,
        CancellationToken ctk = new())
    {
        if (song is null)
            throw new ArgumentNullException(nameof(song));
        ctk.ThrowIfCancellationRequested();

        var audioService = CurrentAudioService;
        if (audioService is not IPreparedAudioTicketService preparedAudioService)
            return null;

        var resource = await ResolveMusicResourceAsync(song, ctk).ConfigureAwait(false);
        if (resource is null)
            return null;

        var ticket = await preparedAudioService
            .GetPreparedAudioTicketAsync(resource, ctk)
            .ConfigureAwait(false);
        if (ctk.IsCancellationRequested)
        {
            await audioService.DisposeAudioTicketAsync(ticket, CancellationToken.None).ConfigureAwait(false);
            ctk.ThrowIfCancellationRequested();
        }
        return new ChopinPreparedPlaybackTicket(this, song, audioService, ticket);
    }

    public override async Task<PreparedPlaybackPromotion?> PromotePreparedPlaybackAsync(
        PreparedPlaybackTicket preparedTicket,
        CancellationToken ctk = new())
    {
        if (preparedTicket is not ChopinPreparedPlaybackTicket incoming
            || !ReferenceEquals(incoming.Owner, this)
            || incoming.IsReleased
            || !SameSong(CurrentSong, incoming.Song)
            || incoming.AudioService is not IPreparedAudioTicketService primaryService)
            return null;

        var oldTicket = CurrentPlayingTicket;
        var oldSong = LastRequestedPlaybackSong;
        CurrentPlayingTicket = incoming.Ticket;
        LastRequestedPlaybackSong = incoming.Song;

        try
        {
            await primaryService
                .SetPrimaryAudioTicketAsync(incoming.Ticket, ctk)
                .ConfigureAwait(false);
            incoming.MarkPromoted();
        }
        catch
        {
            CurrentPlayingTicket = oldTicket;
            LastRequestedPlaybackSong = oldSong;
            throw;
        }

        return new PreparedPlaybackPromotion(
            incoming,
            oldTicket is null || oldSong is null
                ? null
                : new ChopinPreparedPlaybackTicket(this, oldSong, FindAudioServiceForTicket(oldTicket), oldTicket));
    }

    private static bool SameSong(SingleSongBase? left, SingleSongBase? right)
    {
        return ReferenceEquals(left, right)
               || (left is not null
                   && right is not null
                   && left.ProviderId == right.ProviderId
                   && left.TypeId == right.TypeId
                   && left.ActualId == right.ActualId);
    }

    private async Task ReleasePreparedTicketAsync(ChopinPreparedPlaybackTicket lease)
    {
        if (!lease.TryBeginRelease())
            return;

        if (ReferenceEquals(CurrentPlayingTicket, lease.Ticket))
        {
            lease.CancelRelease();
            return;
        }

        try
        {
            var audioService = lease.AudioService
                ?? throw new InvalidOperationException($"The audio service for ticket '{lease.Ticket.AudioServiceId}' is unavailable.");
            await audioService.DisposeAudioTicketAsync(lease.Ticket, CancellationToken.None).ConfigureAwait(false);
            lease.CompleteRelease();
        }
        catch
        {
            lease.CancelRelease();
            throw;
        }
    }

    private sealed class ChopinPreparedPlaybackTicket : PreparedPlaybackTicket
    {
        private int _releaseState;
        private readonly double _targetVolume;

        public ChopinPreparedPlaybackTicket(
            Chopin owner,
            SingleSongBase song,
            AudioServiceBase? audioService,
            AudioTicketBase ticket)
        {
            Owner = owner;
            Song = song;
            AudioService = audioService;
            Ticket = ticket;
            _targetVolume = ticket is IAudioTicketVolumeState volumeState
                ? volumeState.Volume
                : 1d;
        }

        internal Chopin Owner { get; }
        internal AudioServiceBase? AudioService { get; }
        internal AudioTicketBase Ticket { get; }
        internal bool IsReleased => Volatile.Read(ref _releaseState) == 2;

        public override SingleSongBase Song { get; }

        public override double TargetVolume => _targetVolume;

        public override Task PlayAsync(CancellationToken ctk = default)
        {
            ThrowIfReleased();
            return AudioService is IPlayAudioTicketService service
                ? service.PlayAudioTicketAsync(Ticket, ctk)
                : Task.CompletedTask;
        }

        public override Task PauseAsync(CancellationToken ctk = default)
        {
            ThrowIfReleased();
            return AudioService is IPauseAudioTicketService service
                ? service.PauseAudioTicketAsync(Ticket, ctk)
                : Task.CompletedTask;
        }

        public override Task SetVolumeAsync(double volume, CancellationToken ctk = default)
        {
            ThrowIfReleased();
            return AudioService is IAudioTicketVolumeChangeable service
                ? service.ChangeVolumeAsync(Ticket, volume, ctk)
                : Task.CompletedTask;
        }

        public override Task SetPlaybackRateAsync(double playbackRate, CancellationToken ctk = default)
        {
            ThrowIfReleased();
            return AudioService is IPlaybackSpeedChangeable.IPlaybackRateChangeableService service
                ? service.ChangePlaybackSpeedAsync(Ticket, playbackRate, ctk)
                : Task.CompletedTask;
        }

        public override Task DisposeAsync() => Owner.ReleasePreparedTicketAsync(this);

        internal void MarkPromoted()
        {
            ThrowIfReleased();
        }

        internal bool TryBeginRelease() =>
            Interlocked.CompareExchange(ref _releaseState, 1, 0) == 0;

        internal void CancelRelease() => Volatile.Write(ref _releaseState, 0);

        internal void CompleteRelease() => Volatile.Write(ref _releaseState, 2);

        private void ThrowIfReleased()
        {
            if (IsReleased)
                throw new ObjectDisposedException(nameof(PreparedPlaybackTicket));
        }
    }

    public override async Task SeekAsync(long position, CancellationToken ctk = new())
    {
        await EnsureCurrentPlayingTicketAsync(ctk).ConfigureAwait(false);
        if (CurrentPlayingTicket is not { } ticket) return;
        if (FindAudioServiceForTicket(ticket) is IAudioTicketSeekableService seekableService)
            await seekableService.SeekAudioTicketAsync(ticket, position, ctk).ConfigureAwait(false);
    }

    public override async Task PlayAsync(CancellationToken ctk = new())
    {
        await EnsureCurrentPlayingTicketAsync(ctk).ConfigureAwait(false);
        if (CurrentPlayingTicket is not { } ticket) return;
        if (FindAudioServiceForTicket(ticket) is IPlayAudioTicketService playableService)
            await playableService.PlayAudioTicketAsync(ticket, ctk).ConfigureAwait(false);
    }

    public override async Task PauseAsync(CancellationToken ctk = new())
    {
        if (CurrentPlayingTicket is not { } ticket) return;
        if (FindAudioServiceForTicket(ticket) is IPauseAudioTicketService pauseService)
            await pauseService.PauseAudioTicketAsync(ticket, ctk).ConfigureAwait(false);
    }

    public override async Task StopAsync(CancellationToken ctk = new())
    {
        ctk.ThrowIfCancellationRequested();
        await DisposeCurrentPlayingTicketAsync(true, ctk).ConfigureAwait(false);
    }
}
