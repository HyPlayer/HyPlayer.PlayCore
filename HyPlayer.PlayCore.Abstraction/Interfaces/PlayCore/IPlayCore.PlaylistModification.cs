using HyPlayer.PlayCore.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction.Models.SingleItems;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.PlayCore;

public interface IPlayCorePlaylistModification : IPlayCore
{
    public Task ChangeSongContainerAsync(ContainerBase? container, CancellationToken ctk = new());
    public Task InsertSongAsync(SingleSongBase item, int index = -1, CancellationToken ctk = new());
    public Task InsertSongRangeAsync(List<SingleSongBase> items, int index = -1, CancellationToken ctk = new());
    public Task RemoveSongAsync(SingleSongBase item, CancellationToken ctk = new());
    public Task RemoveSongRangeAsync(List<SingleSongBase> item, CancellationToken ctk = new());
    public Task RemoveAllSongAsync(CancellationToken ctk = new());
    public Task SetRandomAsync(bool isRandom, CancellationToken ctk = new());
    public Task ReRandomAsync(CancellationToken ctk = new());
    public Task SetPlayModeAsync(string playModeId, CancellationToken ctk = new());
    public Task SetTransitionAsync(string transitionId, CancellationToken ctk = new());
    public Task<List<SingleSongBase>> GetPlaylistAsync(CancellationToken ctk = new());
    public Task<List<SingleSongBase>> GetOrderedPlaylistAsync(CancellationToken ctk = new());
    public Task<int> GetCurrentIndexAsync(CancellationToken ctk = new());
    public Task<SingleSongBase?> GetSongAtAsync(int index, CancellationToken ctk = new());
    public Task ReversePlaylistAsync(CancellationToken ctk = new());
}
