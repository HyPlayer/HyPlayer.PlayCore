using System.Collections.ObjectModel;
using HyPlayer.PlayCore.Abstraction.Models;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

public interface IProvidableItemRangeProvidable
{
    public Task<ReadOnlyCollection<ProvidableItemBase>> GetProvidableItemsRange(List<string> inProviderIds);
}