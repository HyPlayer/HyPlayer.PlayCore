using HyPlayer.PlayCore.Abstraction.Models;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

/// <summary>
/// Provides provider-owned user library containers such as cloud library, subscriptions, and listening history.
/// </summary>
public interface IUserLibraryProvidable : IProvider
{
    Task<ContainerBase?> GetCurrentUserLibraryContainerAsync(string libraryTypeId, CancellationToken ctk = default);

    Task<ContainerBase?> GetUserLibraryContainerAsync(string userId, string libraryTypeId, CancellationToken ctk = default);
}
