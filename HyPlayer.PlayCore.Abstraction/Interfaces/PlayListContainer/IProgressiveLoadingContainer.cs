using System.Collections.ObjectModel;
using HyPlayer.PlayCore.Abstraction.Models;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.PlayListContainer;

public interface IProgressiveLoadingContainer
{
    public int MaxProgressiveCount { get; }
    public Task<(bool, List<ProvidableItemBase>)> GetProgressiveItemsList(int start, int count);
}