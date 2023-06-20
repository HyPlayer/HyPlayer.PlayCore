using System.Collections.ObjectModel;
using HyPlayer.PlayCore.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction.Models.Songs;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.PlayListContainer;

public interface IProgressiveLoadingContainer
{
    public int MaxProgressiveCount { get; }
    public Task<(bool, ReadOnlyCollection<ProvidableItemBase>)> GetProgressiveItemsList(int start, int count);
}