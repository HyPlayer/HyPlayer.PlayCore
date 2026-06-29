using HyPlayer.PlayCore.Abstraction.Models;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.ProvidableItem;

public interface IHasTrackMetadata : IProvidableItem
{
    string? DiscName { get; }
    int TrackNumber { get; }
}
