using System.Collections.ObjectModel;
using HyPlayer.PlayCore.Abstraction.Models.AudioServiceComponents;
using HyPlayer.PlayCore.Abstraction.Models.Resources;

namespace HyPlayer.PlayCore.Abstraction;

public abstract class AudioServiceBase
{
    public abstract string Id { get; }
    public abstract string Name { get; }
    public abstract Task<AudioTicketBase> GetAudioTicket(MusicResourceBase musicResource);
    public abstract Task DisposeAudioTicket(MusicResourceBase musicResource);
    public abstract Task<List<AudioTicketBase>> GetCreatedAudioTickets();
}