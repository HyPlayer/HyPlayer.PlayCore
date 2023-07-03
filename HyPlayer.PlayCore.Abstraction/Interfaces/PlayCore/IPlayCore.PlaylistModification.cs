using System.Collections.ObjectModel;
using HyPlayer.PlayCore.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction.Models.Songs;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.PlayCore;

public interface IPlayCorePlaylistModification
{
    public Task ChangeSongContainer(ContainerBase? container);
    public Task InsertSong(SingleSongBase item, int index = -1);
    public Task InsertSongRange(List<SingleSongBase> items, int index = -1);
    public Task RemoveSong(SingleSongBase item);
    public Task RemoveSongRange(List<SingleSongBase> item);
    public Task RemoveAllSong();
    public Task SetRandom(bool isRandom);
    public Task ReRandom();
}