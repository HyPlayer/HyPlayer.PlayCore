using HyPlayer.PlayCore.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction.Models.SingleItems;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

/// <summary>
/// Provides comment entry points for provider-level comment surfaces.
/// </summary>
public interface ICommentProvidable : IProvider
{
    /// <summary>
    /// Gets the provider's default comment container when one exists.
    /// </summary>
    /// <param name="ctk">The cancellation token for the operation.</param>
    /// <returns>The default provider comment container.</returns>
    public Task<ContainerBase?> GetCommentContainerAsync(CancellationToken ctk = new());

    /// <summary>
    /// Gets a page of comments associated with a providable item.
    /// </summary>
    /// <param name="itemId">The provider-scoped item id that owns the comments.</param>
    /// <param name="typeId">The provider-neutral type id of the commented item.</param>
    /// <param name="offset">The zero-based comment offset to request.</param>
    /// <param name="count">The maximum number of comments to request.</param>
    /// <param name="sortType">Provider-neutral comment sort type. 1=recommended, 2=hot, 3=time.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    /// <returns>A provider-neutral comment page.</returns>
    public Task<ProviderPageResult<CommentBase>> GetCommentsAsync(string itemId, string typeId, int offset, int count, int sortType = 1, CancellationToken ctk = new());

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
    /// Gets a page of replies threaded under a provider comment.
    /// </summary>
    /// <param name="itemId">The provider-scoped item id that owns the comment.</param>
    /// <param name="typeId">The provider-neutral type id of the commented item.</param>
    /// <param name="commentId">The provider-scoped parent comment id.</param>
    /// <param name="offset">The zero-based reply offset to request.</param>
    /// <param name="count">The maximum number of replies to request.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    /// <returns>A provider-neutral reply page.</returns>
    public Task<ProviderPageResult<CommentBase>> GetThreadedCommentsAsync(string itemId, string typeId, string commentId, int offset, int count, CancellationToken ctk = new());

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
