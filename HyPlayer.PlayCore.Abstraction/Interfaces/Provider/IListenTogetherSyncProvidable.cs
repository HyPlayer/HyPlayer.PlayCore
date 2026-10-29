using HyPlayer.PlayCore.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction.Models.SingleItems;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

public interface IListenTogetherSyncProvidable : IListenTogetherProvidable
{
    public Task<ProviderListenTogetherStatus> JoinListenTogetherRoomAsync(string roomId, string inviterId, CancellationToken ctk = new());

    public Task<TimeSpan?> SendListenTogetherHeartbeatAsync(string roomId, ProviderListenTogetherHeartbeat heartbeat, CancellationToken ctk = new());

    public Task<ProviderListenTogetherRoomState> GetListenTogetherRoomStateAsync(string roomId, CancellationToken ctk = new());

    public Task<List<SingleSongBase>> GetListenTogetherQueueSongsAsync(IReadOnlyList<string> itemIds, CancellationToken ctk = new());

    public Task EndListenTogetherRoomAsync(string roomId, CancellationToken ctk = new());
}
