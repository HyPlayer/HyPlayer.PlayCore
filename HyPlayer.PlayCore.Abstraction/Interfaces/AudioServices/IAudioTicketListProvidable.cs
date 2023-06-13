using System.Collections.ObjectModel;
using HyPlayer.PlayCore.Abstraction.Models.AudioServiceComponents;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.AudioServices;

public interface IAudioTicketListProvidable
{
    public abstract Task<ReadOnlyCollection<AudioTicketBase>> GetAudioTicketListAsync();
}