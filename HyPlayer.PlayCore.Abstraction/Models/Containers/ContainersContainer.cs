using System.Collections.ObjectModel;

namespace HyPlayer.PlayCore.Abstraction.Models.Containers;

public abstract class ContainersContainer : SongContainerBase
{
    public abstract Task<ReadOnlyCollection<SongContainerBase>> GetSubContainer();
}