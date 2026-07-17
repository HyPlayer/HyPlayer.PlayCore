using HyPlayer.PlayCore.Abstraction.Models.AudioServiceComponents;
using HyPlayer.PlayCore.Abstraction.Models.Resources;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.AudioServices;

/// <summary>Creates a ticket without changing the primary playback source.</summary>
public interface IPreparedAudioTicketService : IAudioService
{
    Task<AudioTicketBase> GetPreparedAudioTicketAsync(
        MusicResourceBase musicResource,
        CancellationToken ctk = default);

    Task SetPrimaryAudioTicketAsync(
        AudioTicketBase ticket,
        CancellationToken ctk = default);
}

public interface IAudioTicketVolumeState
{
    double Volume { get; set; }
}
