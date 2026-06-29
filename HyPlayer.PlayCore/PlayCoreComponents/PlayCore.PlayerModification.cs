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

        var resource = await ResolveMusicResourceAsync(requestedSong, ctk).ConfigureAwait(false);
        if (resource is null)
        {
            await PublishPlaybackFailureAsync(requestedSong, "No matching music resource provider returned a resource.", null, ctk).ConfigureAwait(false);
            return null;
        }

        if (!ReferenceEquals(CurrentSong, requestedSong) || !ReferenceEquals(CurrentAudioService, audioService))
            return null;

        var ticket = await audioService.GetAudioTicketAsync(resource, ctk).ConfigureAwait(false);
        if (!ReferenceEquals(CurrentSong, requestedSong) || !ReferenceEquals(CurrentAudioService, audioService))
        {
            await audioService.DisposeAudioTicketAsync(ticket, ctk).ConfigureAwait(false);
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
        if (CurrentPlayingTicket is not { } ticket)
            return;

        try
        {
            var audioService = FindAudioServiceForTicket(ticket);
            if (audioService is null)
                return;

            if (stopTicket && audioService is IStopAudioTicketService stopService)
                await stopService.StopTicketAsync(ticket, ctk).ConfigureAwait(false);

            await audioService.DisposeAudioTicketAsync(ticket, ctk).ConfigureAwait(false);
        }
        finally
        {
            if (ReferenceEquals(CurrentPlayingTicket, ticket))
                ClearCurrentPlayingTicket();
        }
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
        await DisposeCurrentPlayingTicketAsync(true, ctk).ConfigureAwait(false);
    }
}
