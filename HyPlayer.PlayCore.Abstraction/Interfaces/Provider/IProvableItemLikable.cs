using System.Collections.ObjectModel;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

public interface IProvableItemLikable
{
    public Task LikeProvidableItem(string inProviderId, string? targetId);
    public Task UnlikeProvidableItem(string inProviderId, string? targetId);
    public Task<List<string>> GetLikedProvidableIds(string typeId);
}