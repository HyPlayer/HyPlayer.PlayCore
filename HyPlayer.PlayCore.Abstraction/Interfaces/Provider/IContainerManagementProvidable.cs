using HyPlayer.PlayCore.Abstraction.Models;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

/// <summary>
/// Provides provider-neutral management of playlist-like containers.
/// </summary>
public interface IContainerManagementProvidable : IProvider
{
    /// <summary>
    /// Creates a provider-owned container.
    /// </summary>
    /// <param name="name">The display name for the new container.</param>
    /// <param name="isPrivate">Whether the container should be private when the provider supports privacy.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    /// <returns>The created container.</returns>
    public Task<ContainerBase> CreateContainerAsync(string name, bool isPrivate, CancellationToken ctk = new());

    /// <summary>
    /// Deletes a provider-owned container.
    /// </summary>
    /// <param name="containerId">The provider-scoped container id.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    public Task DeleteContainerAsync(string containerId, CancellationToken ctk = new());

    /// <summary>
    /// Updates the privacy state of a provider-owned container.
    /// </summary>
    /// <param name="containerId">The provider-scoped container id.</param>
    /// <param name="isPrivate">Whether the container should be private.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    public Task SetContainerPrivacyAsync(string containerId, bool isPrivate, CancellationToken ctk = new());

}
