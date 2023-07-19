using System.Collections.ObjectModel;
using HyPlayer.PlayCore.Abstraction.Interfaces.PlayListController;
using HyPlayer.PlayCore.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction.Models.Songs;

namespace HyPlayer.PlayCore;

public sealed partial class Chopin
{
    private ContainerBase? _currentSongContainer;
    private ObservableCollection<SingleSongBase>? _songList;
    private bool _isRandom;

    public override bool IsRandom
    {
        get => _isRandom;
        protected set => SetField(ref _isRandom, value);
    }

    public override ContainerBase? CurrentSongContainer
    {
        get => _currentSongContainer;
        protected set => SetField(ref _currentSongContainer, value);
    }

    public override ObservableCollection<SingleSongBase>? SongList
    {
        get => _songList;
        protected set => SetField(ref _songList, value);
    }

    public override async Task ChangeSongContainer(ContainerBase? container)
    {
        CurrentSongContainer = container;
    }

    public override async Task InsertSong(SingleSongBase item, int index = -1)
    {
        if (_currentPlayListController is IInsertablePlaylistController controller)
            await controller.InsertSong(item, index);
    }

    public override async Task InsertSongRange(List<SingleSongBase> items, int index = -1)
    {
        if (_currentPlayListController is IRangeControllablePlayListController controller)
            await controller.InsertSongRange(items, index);
    }

    public override async Task RemoveSong(SingleSongBase item)
    {
        if (_currentPlayListController is IInsertablePlaylistController controller)
            await controller.RemoveSong(item);
    }

    public override async Task RemoveSongRange(List<SingleSongBase> items)
    {
        if (_currentPlayListController is IRangeControllablePlayListController controller)
            await controller.RemoveSongRange(items);
    }

    public override async Task RemoveAllSong()
    {
        if (_currentPlayListController is not null)
            await _currentPlayListController.ClearSongs();
    }

    public override async Task SetRandom(bool isRandom)
    {
        if (_currentSongContainer is IRandomizablePlayListController randomizablePlayListController)
            await randomizablePlayListController.Randomize(isRandom ? -1 : DateTime.Now.Millisecond);
    }

    public override async Task ReRandom()
    {
        if (_currentSongContainer is IRandomizablePlayListController randomizablePlayListController)
            await randomizablePlayListController.Randomize(DateTime.Now.Millisecond);
    }
}