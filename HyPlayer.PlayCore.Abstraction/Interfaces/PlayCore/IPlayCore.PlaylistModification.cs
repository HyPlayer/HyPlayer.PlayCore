using System.Collections.ObjectModel;
using HyPlayer.PlayCore.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction.Models.Songs;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.PlayCore;

public interface IPlayCorePlaylistModification
{
    public Task ChangeSongContainer(SongContainerBase? container);
    public Task InsertSong(SingleSongBase item, int index = -1);
    public Task InsertSongRange(ReadOnlyCollection<SingleSongBase> items, int index = -1);
    public Task RemoveSong(SingleSongBase item);
    public Task RemoveSongRange(ReadOnlyCollection<SingleSongBase> item);
    public Task RemoveAllSong();
    public Task SetRandom(bool isRandom);
    public Task ReRandom();
}