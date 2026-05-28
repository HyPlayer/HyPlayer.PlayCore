namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

public interface IProvableItemLikable : IProvider
{
    Task LikeProvidableItemAsync(string inProviderId, string? targetId, CancellationToken ctk = default(CancellationToken));

    Task UnlikeProvidableItemAsync(string inProviderId, string? targetId, CancellationToken ctk = default(CancellationToken));

    Task<List<string>> GetLikedProvidableIdsAsync(string typeId, CancellationToken ctk = default(CancellationToken));
}
