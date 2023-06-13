using System.Collections.ObjectModel;
using HyPlayer.PlayCore.Abstraction.Models.AudioServiceComponents;
using HyPlayer.PlayCore.Abstraction.Models.Resources;

namespace HyPlayer.PlayCore.Abstraction;

public abstract class AudioServiceBase
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public abstract Task<AudioTicketBase> GetAudioTicket(MusicResourceBase musicResource);
    public abstract Task DisposeAudioTicket(MusicResourceBase musicResource);
    public abstract Task<ReadOnlyCollection<AudioTicketBase>> GetCreatedAudioTickets();
}