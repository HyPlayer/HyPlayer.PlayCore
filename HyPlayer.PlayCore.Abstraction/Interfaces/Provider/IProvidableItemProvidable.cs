using HyPlayer.PlayCore.Abstraction.Models;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

public interface IProvidableItemProvidable
{
    public Task<ProvidableItemBase> GetProvidableItemById(string inProviderId);
}