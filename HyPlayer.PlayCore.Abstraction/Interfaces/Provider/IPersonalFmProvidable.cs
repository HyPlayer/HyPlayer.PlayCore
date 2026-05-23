using HyPlayer.PlayCore.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction.Models.Containers;
using HyPlayer.PlayCore.Abstraction.Models.SingleItems;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

/// <summary>
/// Provides provider-neutral Personal FM and feedback operations.
/// </summary>
public interface IPersonalFmProvidable : IProvider
{
    /// <summary>
    /// Gets the next provider-curated Personal FM queue for the current session.
    /// </summary>
    /// <param name="ctk">The cancellation token for the operation.</param>
    /// <returns>The next Personal FM songs.</returns>
    public Task<List<SingleSongBase>> GetPersonalFmQueueAsync(CancellationToken ctk = new());

    /// <summary>
    /// Marks a Personal FM song as unwanted for the current provider session.
    /// </summary>
    /// <param name="songId">The provider-scoped song id.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    public Task TrashPersonalFmSongAsync(string songId, CancellationToken ctk = new());

    /// <summary>
    /// Gets provider-generated AI DJ or Personal FM contextual recommendations for a song.
    /// </summary>
    /// <param name="songId">The provider-scoped song id.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    /// <returns>A provider-neutral recommendation container.</returns>
    public Task<ContainerBase> GetPersonalFmContextAsync(string songId, CancellationToken ctk = new());
}
