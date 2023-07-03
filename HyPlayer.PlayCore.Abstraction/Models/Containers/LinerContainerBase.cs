using System.Collections.ObjectModel;
using HyPlayer.PlayCore.Abstraction.Models.Songs;

namespace HyPlayer.PlayCore.Abstraction.Models.Containers;

public abstract class LinerContainerBase : ContainerBase
{
    public abstract Task<List<ProvidableItemBase>> GetAllItems();
}