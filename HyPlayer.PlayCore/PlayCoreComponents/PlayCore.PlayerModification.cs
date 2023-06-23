using HyPlayer.PlayCore.Abstraction.Interfaces.AudioServices;
using HyPlayer.PlayCore.Abstraction.Models.AudioServiceComponents;

namespace HyPlayer.PlayCore;

public partial class Chopin
{
    private AudioTicketBase? _currentPlayingTicket;

    public override AudioTicketBase? CurrentPlayingTicket
    {
        get => _currentPlayingTicket;
        protected set => SetField(ref _currentPlayingTicket, value);
    }

    public override async Task Seek(long position)
    {
        if (CurrentPlayingTicket is null) return;
        if (CurrentPlayingTicket?.AudioServiceId == CurrentAudioService?.Id)
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (CurrentAudioService is IAudioTicketSeekableService seekableService)
                await seekableService.SeekAudioTicket(CurrentPlayingTicket!, position);
    }

    public override async Task Play()
    {
        if (CurrentPlayingTicket is null) return;
        if (CurrentPlayingTicket?.AudioServiceId == CurrentAudioService?.Id)
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (CurrentAudioService is IPlayAudioTicketService playableService)
                await playableService.PlayAudioTicket(CurrentPlayingTicket!);
    }

    public override async Task Pause()
    {
        if (CurrentPlayingTicket is null) return;
        if (CurrentPlayingTicket?.AudioServiceId == CurrentAudioService?.Id)
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (CurrentAudioService is IPauseAudioTicketService pauseService)
                await pauseService.PauseAudioTicket(CurrentPlayingTicket!);
    }

    public override async Task Stop()
    {
        if (CurrentPlayingTicket is null) return;
        if (CurrentPlayingTicket?.AudioServiceId == CurrentAudioService?.Id)
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (CurrentAudioService is IStopAudioTicketService stopService)
                await stopService.StopTicket(CurrentPlayingTicket!);
    }
}