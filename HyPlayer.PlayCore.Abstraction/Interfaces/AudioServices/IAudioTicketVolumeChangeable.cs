using HyPlayer.PlayCore.Abstraction.Models.AudioServiceComponents;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.AudioServices;

public interface IAudioTicketVolumeChangeable
{
    public Task ChangeVolume(AudioTicketBase ticket, double volume);
}