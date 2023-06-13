using HyPlayer.PlayCore.Abstraction.Models.AudioServiceComponents;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.AudioServices;

public interface IPlayAudioTicketService
{
    public Task PlayAudioTicket(AudioTicketBase ticket);
}