using HyPlayer.PlayCore.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction.Models.Containers;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

/// <summary>
/// Provides provider-neutral cloud upload operations.
/// </summary>
public interface ICloudUploadProvidable : IProvider
{
    /// <summary>
    /// Uploads a local media resource into the provider cloud library.
    /// </summary>
    /// <param name="resource">The provider-neutral resource to upload.</param>
    /// <param name="metadata">Provider-neutral metadata to associate with the uploaded item.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    /// <returns>The cloud-library item created by the provider.</returns>
    public Task<CloudLibraryItemBase> UploadCloudLibraryItemAsync(ResourceBase resource, IReadOnlyDictionary<string, string> metadata, CancellationToken ctk = new());
}
