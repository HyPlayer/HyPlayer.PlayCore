using HyPlayer.PlayCore.Abstraction.Models.AudioServiceComponents;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.AudioServices;

public interface IStopAudioTicketService
{
    public Task StopTicket(AudioTicketBase ticket);
}