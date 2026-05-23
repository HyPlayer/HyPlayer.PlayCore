using HyPlayer.PlayCore.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction.Models.SingleItems;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

/// <summary>
/// Provides provider-neutral listen-together room and synchronization operations.
/// </summary>
public interface IListenTogetherProvidable : IProvider
{
    /// <summary>
    /// Creates a provider listen-together room for the supplied queue.
    /// </summary>
    /// <param name="queue">The initial queue to share with the room.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    /// <returns>The provider-scoped room id.</returns>
    public Task<string> CreateListenTogetherRoomAsync(List<SingleSongBase> queue, CancellationToken ctk = new());

    /// <summary>
    /// Checks whether a provider listen-together room can be joined.
    /// </summary>
    /// <param name="roomId">The provider-scoped room id.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    /// <returns><see langword="true" /> when the room can be joined; otherwise, <see langword="false" />.</returns>
    public Task<bool> CanJoinListenTogetherRoomAsync(string roomId, CancellationToken ctk = new());

    /// <summary>
    /// Sends a provider-neutral playback command to a listen-together room.
    /// </summary>
    /// <param name="roomId">The provider-scoped room id.</param>
    /// <param name="command">The provider-neutral playback command.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    public Task SendListenTogetherPlaybackCommandAsync(string roomId, ProviderListenTogetherPlaybackCommand command, CancellationToken ctk = new());

    /// <summary>
    /// Reports the current shared queue to a listen-together room.
    /// </summary>
    /// <param name="roomId">The provider-scoped room id.</param>
    /// <param name="report">The current room queue report.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    public Task ReportListenTogetherQueueAsync(string roomId, ProviderListenTogetherQueueReport report, CancellationToken ctk = new());

    /// <summary>
    /// Gets the provider-neutral status for a listen-together room.
    /// </summary>
    /// <param name="roomId">The provider-scoped room id.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    /// <returns>The provider-neutral room status.</returns>
    public Task<ProviderListenTogetherStatus?> GetListenTogetherStatusAsync(string roomId, CancellationToken ctk = new());
}
