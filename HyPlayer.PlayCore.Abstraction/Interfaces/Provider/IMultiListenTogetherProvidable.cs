using HyPlayer.PlayCore.Abstraction.Models;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

public interface IMultiListenTogetherProvidable : IProvider
{
    public Task<ProviderMultiListenTogetherRoom> CreateMultiListenTogetherRoomAsync(string seedItemId, CancellationToken ctk = new());

    public Task<ProviderMultiListenTogetherRoom> JoinMultiListenTogetherRoomAsync(string roomId, string inviterId, CancellationToken ctk = new());

    public Task<ProviderMultiListenTogetherRoom?> GetMultiListenTogetherRoomAsync(CancellationToken ctk = new());

    public Task<ProviderMultiListenTogetherHeartbeat> SendMultiListenTogetherHeartbeatAsync(string roomId, CancellationToken ctk = new());

    public Task<List<ProviderMultiListenTogetherQueueItem>> GetMultiListenTogetherQueueAsync(string roomId, CancellationToken ctk = new());

    public Task<ProviderMultiListenTogetherOperation> RecommendMultiListenTogetherItemAsync(string roomId, string itemId, CancellationToken ctk = new());

    public Task<ProviderMultiListenTogetherOperation> RequestMultiListenTogetherSkipAsync(string roomId, string itemId, string entryId, CancellationToken ctk = new());

    public Task LeaveMultiListenTogetherRoomAsync(string roomId, CancellationToken ctk = new());
}
