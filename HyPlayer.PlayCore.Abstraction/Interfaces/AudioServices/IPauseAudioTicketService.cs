using HyPlayer.PlayCore.Abstraction.Models.AudioServiceComponents;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.AudioServices;

public interface IPauseAudioTicketService
{
    public Task PauseAudioTicket(AudioTicketBase ticket);
}