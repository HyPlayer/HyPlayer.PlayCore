using HyPlayer.PlayCore.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction.Models.SingleItems;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

public interface IProvidableItemCommentPageTokenProvidable : IProvider
{
    public Task<ProviderPageResult<CommentBase>> GetCommentsPageAsync(string itemId, string typeId, string? pageToken, int count, int sortType = 1, CancellationToken ctk = new());

    public Task<ProviderPageResult<CommentBase>> GetThreadedCommentsPageAsync(string itemId, string typeId, string commentId, string? pageToken, int count, CancellationToken ctk = new());
}
