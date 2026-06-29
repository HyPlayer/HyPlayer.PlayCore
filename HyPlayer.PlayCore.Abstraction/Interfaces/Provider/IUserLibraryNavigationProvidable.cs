using HyPlayer.PlayCore.Abstraction.Models;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

/// <summary>
/// Provides provider-defined current-user library groups for app navigation surfaces.
/// </summary>
public interface IUserLibraryNavigationProvidable : IProvider
{
    Task<IReadOnlyList<ProviderLibraryNavigationGroup>> GetCurrentUserLibraryNavigationGroupsAsync(CancellationToken ctk = default);
}
