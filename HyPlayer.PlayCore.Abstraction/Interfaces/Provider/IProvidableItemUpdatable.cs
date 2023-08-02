using HyPlayer.PlayCore.Abstraction.Models;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

public interface IProvidableItemUpdatable
{
    public Task<ProvidableItemBase?> UpdateProvidableItemInfo(ProvidableItemBase providableItem);
}