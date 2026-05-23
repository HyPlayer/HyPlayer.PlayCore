using HyPlayer.PlayCore.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction.Models.SingleItems;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

/// <summary>
/// Provides provider-neutral comment operations for providable items.
/// </summary>
public interface IProvidableItemCommentProvidable : IProvider
{
    /// <summary>
    /// Gets a page of comments associated with a providable item.
    /// </summary>
    /// <param name="itemId">The provider-scoped item id that owns the comments.</param>
    /// <param name="typeId">The provider-neutral type id of the commented item.</param>
    /// <param name="offset">The zero-based comment offset to request.</param>
    /// <param name="count">The maximum number of comments to request.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    /// <returns>A provider-neutral comment page.</returns>
    public Task<ProviderPageResult<CommentBase>> GetCommentsAsync(string itemId, string typeId, int offset, int count, CancellationToken ctk = new());

    /// <summary>
    /// Gets a page of threaded replies for a comment.
    /// </summary>
    /// <param name="itemId">The provider-scoped item id that owns the comments.</param>
    /// <param name="typeId">The provider-neutral type id of the commented item.</param>
    /// <param name="commentId">The provider-scoped comment id.</param>
    /// <param name="offset">The zero-based reply offset to request.</param>
    /// <param name="count">The maximum number of replies to request.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    /// <returns>A provider-neutral page of threaded comment replies.</returns>
    public Task<ProviderPageResult<CommentBase>> GetThreadedCommentsAsync(string itemId, string typeId, string commentId, int offset, int count, CancellationToken ctk = new());

    /// <summary>
    /// Posts a comment to a providable item.
    /// </summary>
    /// <param name="itemId">The provider-scoped item id that receives the comment.</param>
    /// <param name="typeId">The provider-neutral type id of the commented item.</param>
    /// <param name="content">The comment content.</param>
    /// <param name="replyToCommentId">The provider-scoped comment id to reply to, or <see langword="null" /> for a top-level comment.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    /// <returns>The created comment when the provider returns one.</returns>
    public Task<CommentBase?> PostCommentAsync(string itemId, string typeId, string content, string? replyToCommentId = null, CancellationToken ctk = new());

    /// <summary>
    /// Likes or unlikes a provider comment for the current session.
    /// </summary>
    /// <param name="itemId">The provider-scoped item id that owns the comment.</param>
    /// <param name="typeId">The provider-neutral type id of the commented item.</param>
    /// <param name="commentId">The provider-scoped comment id.</param>
    /// <param name="like">Whether the comment should be liked or unliked.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    public Task SetCommentLikeStateAsync(string itemId, string typeId, string commentId, bool like, CancellationToken ctk = new());
}
