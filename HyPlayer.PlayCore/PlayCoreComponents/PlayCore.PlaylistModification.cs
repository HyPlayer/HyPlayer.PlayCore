using System.Collections.ObjectModel;
using HyPlayer.PlayCore.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction.Models.Songs;

namespace HyPlayer.PlayCore;

public partial class Chopin
{
    private SongContainerBase? _currentSongContainer;
    private ReadOnlyCollection<SingleSongBase>? _songList;

    public new SongContainerBase? CurrentSongContainer
    {
        get => _currentSongContainer;
        private set => SetField(ref _currentSongContainer, value);
    }

    public new ReadOnlyCollection<SingleSongBase>? SongList
    {
        get => _songList;
        private set => SetField(ref _songList, value);
    }

    public override async Task ChangeSongContainer(SongContainerBase? container)
    {
        CurrentSongContainer = container;
    }

    public override async Task InsertSong(SingleSongBase item, int index = -1)
    {
        if (_currentPlayListController is not null)
            await _currentPlayListController.InsertSong(item, index);
    }

    public override async Task InsertSongRange(ReadOnlyCollection<SingleSongBase> items, int index = -1)
    {
        if (_currentPlayListController is not null)
            await _currentPlayListController.InsertSongRange(items, index);
    }

    public override async Task RemoveSong(SingleSongBase item)
    {
        if (_currentPlayListController is not null)
            await _currentPlayListController.RemoveSong(item);
    }

    public override async Task RemoveSongRange(ReadOnlyCollection<SingleSongBase> items)
    {
        if (_currentPlayListController is not null)
            await _currentPlayListController.RemoveSongRange(items);
    }

    public override async Task RemoveAllSong()
    {
        if (_currentPlayListController is not null)
            await _currentPlayListController.ClearSongs();
    }
}