namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

public interface IDynamicCoverProvidable : IProvider
{
    public Task<Uri?> GetDynamicCoverAsync(string itemId, CancellationToken ctk = new());
}
