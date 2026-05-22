using HyPlayer.PlayCore.Abstraction.Interfaces.AudioServices;
using HyPlayer.PlayCore.Abstraction.Interfaces.Provider;
using HyPlayer.PlayCore.Abstraction;
using HyPlayer.PlayCore.Abstraction.Models.AudioServiceComponents;
using HyPlayer.PlayCore.Abstraction.Models.Resources;
using HyPlayer.PlayCore.Abstraction.Models.SingleItems;

namespace HyPlayer.PlayCore;

public sealed partial class Chopin
{
    public override AudioTicketBase? CurrentPlayingTicket { get; protected set; }

    private async Task<AudioTicketBase?> EnsureCurrentPlayingTicketAsync(CancellationToken ctk)
    {
        if (CurrentSong is null || CurrentAudioService is null)
            return CurrentPlayingTicket;

        if (CurrentPlayingTicket?.AudioServiceId == CurrentAudioService.Id
            && ReferenceEquals(LastRequestedPlaybackSong, CurrentSong))
            return CurrentPlayingTicket;

        var resource = await ResolveMusicResourceAsync(CurrentSong, ctk).ConfigureAwait(false);
        if (resource is null)
            return CurrentPlayingTicket;

        if (CurrentPlayingTicket is { } oldTicket
            && oldTicket.AudioServiceId == CurrentAudioService.Id)
            await CurrentAudioService.DisposeAudioTicketAsync(oldTicket, ctk).ConfigureAwait(false);

        CurrentPlayingTicket = await CurrentAudioService.GetAudioTicketAsync(resource, ctk).ConfigureAwait(false);
        LastRequestedPlaybackSong = CurrentSong;
        return CurrentPlayingTicket;
    }

    private async Task<MusicResourceBase?> ResolveMusicResourceAsync(SingleSongBase song, CancellationToken ctk)
    {
        var provider = MusicProviders?
            .OfType<ProviderBase>()
            .OfType<IMusicResourceProvidable>()
            .FirstOrDefault(p => p is ProviderBase providerBase && providerBase.Id == song.ProviderId);

        if (provider is null)
            return null;

        return await provider.GetMusicResourceAsync(song, null!, ctk).ConfigureAwait(false);
    }

    public override async Task SeekAsync(long position, CancellationToken ctk = new())
    {
        await EnsureCurrentPlayingTicketAsync(ctk).ConfigureAwait(false);
        if (CurrentPlayingTicket is null) return;
        if (CurrentPlayingTicket?.AudioServiceId == CurrentAudioService?.Id)
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (CurrentAudioService is IAudioTicketSeekableService seekableService)
                await seekableService.SeekAudioTicketAsync(CurrentPlayingTicket!, position, ctk).ConfigureAwait(false);
    }

    public override async Task PlayAsync(CancellationToken ctk = new())
    {
        await EnsureCurrentPlayingTicketAsync(ctk).ConfigureAwait(false);
        if (CurrentPlayingTicket is null) return;
        if (CurrentPlayingTicket?.AudioServiceId == CurrentAudioService?.Id)
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (CurrentAudioService is IPlayAudioTicketService playableService)
                await playableService.PlayAudioTicketAsync(CurrentPlayingTicket!, ctk).ConfigureAwait(false);
    }

    public override async Task PauseAsync(CancellationToken ctk = new())
    {
        if (CurrentPlayingTicket is null) return;
        if (CurrentPlayingTicket?.AudioServiceId == CurrentAudioService?.Id)
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (CurrentAudioService is IPauseAudioTicketService pauseService)
                await pauseService.PauseAudioTicketAsync(CurrentPlayingTicket!, ctk).ConfigureAwait(false);
    }

    public override async Task StopAsync(CancellationToken ctk = new())
    {
        if (CurrentPlayingTicket is null) return;
        if (CurrentPlayingTicket?.AudioServiceId == CurrentAudioService?.Id)
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (CurrentAudioService is IStopAudioTicketService stopService)
            {
                await stopService.StopTicketAsync(CurrentPlayingTicket!, ctk).ConfigureAwait(false);
                ClearCurrentPlayingTicket();
            }
    }
}
