using HyPlayer.PlayCore.Abstraction.Models;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.ProvidableItem;

public interface IHasPublishTime : IProvidableItem
{
    long PublishTime { get; set; }
}
