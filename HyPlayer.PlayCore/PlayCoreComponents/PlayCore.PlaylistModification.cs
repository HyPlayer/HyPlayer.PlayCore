using System.Collections.ObjectModel;
using HyPlayer.PlayCore.Abstraction.Interfaces.PlayListController;
using HyPlayer.PlayCore.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction.Models.SingleItems;

namespace HyPlayer.PlayCore;

public sealed partial class Chopin
{
    public override bool IsRandom { get; protected set; }

    public override ContainerBase? CurrentSongContainer { get; protected set; }

    public override List<SingleSongBase>? SongList { get; protected set; }

    public override Task ChangeSongContainer(ContainerBase? container)
    {
        CurrentSongContainer = container;
        return Task.CompletedTask;
    }

    public override async Task InsertSong(SingleSongBase item, int index = -1)
    {
        if (CurrentPlayListController is IInsertablePlaylistController controller)
            await controller.InsertSong(item, index);
    }

    public override async Task InsertSongRange(List<SingleSongBase> items, int index = -1)
    {
        if (CurrentPlayListController is IRangeControllablePlayListController controller)
            await controller.InsertSongRange(items, index);
    }

    public override async Task RemoveSong(SingleSongBase item)
    {
        if (CurrentPlayListController is IInsertablePlaylistController controller)
            await controller.RemoveSong(item);
    }

    public override async Task RemoveSongRange(List<SingleSongBase> items)
    {
        if (CurrentPlayListController is IRangeControllablePlayListController controller)
            await controller.RemoveSongRange(items);
    }

    public override async Task RemoveAllSong()
    {
        if (CurrentPlayListController is not null)
            await CurrentPlayListController.ClearSongs();
    }

    public override async Task SetRandom(bool isRandom)
    {
        if (CurrentSongContainer is IRandomizablePlayListController randomizablePlayListController)
            await randomizablePlayListController.Randomize(isRandom ? -1 : DateTime.Now.Millisecond);
    }

    public override async Task ReRandom()
    {
        if (CurrentPlayListController is IRandomizablePlayListController randomizablePlayListController)
            await randomizablePlayListController.Randomize(DateTime.Now.Millisecond);
    }
}