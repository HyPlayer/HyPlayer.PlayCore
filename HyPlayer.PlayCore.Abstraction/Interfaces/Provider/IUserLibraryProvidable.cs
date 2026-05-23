using HyPlayer.PlayCore.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction.Models.Containers;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

/// <summary>
/// Provides provider-neutral user library and cloud collection operations.
/// </summary>
public interface IUserLibraryProvidable : IProvider
{
    /// <summary>
    /// Gets the current or specified provider user as a providable item.
    /// </summary>
    /// <param name="userId">The provider-scoped user id, or <see langword="null" /> for the current session user.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    /// <returns>The user item when one is available.</returns>
    public Task<ProvidableItemBase?> GetUserAsync(string? userId = null, CancellationToken ctk = new());

    /// <summary>
    /// Gets provider containers owned by or subscribed by a user.
    /// </summary>
    /// <param name="userId">The provider-scoped user id, or <see langword="null" /> for the current session user.</param>
    /// <param name="offset">The zero-based offset to request.</param>
    /// <param name="count">The maximum number of containers to request.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    /// <returns>A provider-neutral page of user containers.</returns>
    public Task<ProviderPageResult<ContainerBase>> GetUserContainersAsync(string? userId, int offset, int count, CancellationToken ctk = new());

    /// <summary>
    /// Gets a provider-specific library collection, such as followed artists, albums, or radio channels.
    /// </summary>
    /// <param name="typeId">The provider-neutral item type id to list.</param>
    /// <param name="offset">The zero-based offset to request.</param>
    /// <param name="count">The maximum number of items to request.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    /// <returns>A provider-neutral page of library items.</returns>
    public Task<ProviderPageResult<ProvidableItemBase>> GetUserLibraryItemsAsync(string typeId, int offset, int count, CancellationToken ctk = new());

    /// <summary>
    /// Gets provider listening history for a user.
    /// </summary>
    /// <param name="userId">The provider-scoped user id.</param>
    /// <param name="rangeId">A provider-neutral range id such as all-time or recent.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    /// <returns>A provider-neutral page of historical items.</returns>
    public Task<List<ProvidableItemBase>> GetUserListeningHistoryAsync(string userId, string rangeId, CancellationToken ctk = new());

    /// <summary>
    /// Gets one page of provider cloud-library items.
    /// </summary>
    /// <param name="offset">The zero-based offset to request.</param>
    /// <param name="count">The maximum number of cloud items to request.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    /// <returns>A provider-neutral page of cloud-library items.</returns>
    public Task<ProviderPageResult<CloudLibraryItemBase>> GetCloudLibraryItemsAsync(int offset, int count, CancellationToken ctk = new());

    /// <summary>
    /// Deletes a provider cloud-library item.
    /// </summary>
    /// <param name="itemId">The provider-scoped cloud item id.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    public Task DeleteCloudLibraryItemAsync(string itemId, CancellationToken ctk = new());
}
