using System.Collections.ObjectModel;
using HyPlayer.PlayCore.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction.Models.SingleItems;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.PlayCore;

public interface IPlayCorePlaylistModification
{
    public Task ChangeSongContainerAsync(ContainerBase? container);
    public Task InsertSongAsync(SingleSongBase item, int index = -1);
    public Task InsertSongRangeAsync(List<SingleSongBase> items, int index = -1);
    public Task RemoveSongAsync(SingleSongBase item);
    public Task RemoveSongRangeAsync(List<SingleSongBase> item);
    public Task RemoveAllSongAsync();
    public Task SetRandomAsync(bool isRandom);
    public Task ReRandomAsync();
}