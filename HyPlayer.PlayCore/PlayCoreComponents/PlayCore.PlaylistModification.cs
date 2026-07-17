using HyPlayer.PlayCore.Abstraction;
using HyPlayer.PlayCore.Abstraction.Interfaces.PlayListController;
using HyPlayer.PlayCore.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction.Models.SingleItems;

namespace HyPlayer.PlayCore;

public sealed partial class Chopin
{
    public override bool IsRandom { get; protected set; }
    public override string ActivePlayModeId { get; protected set; } = "seq";

    public override ContainerBase? CurrentSongContainer { get; protected set; }

    public override PlayListManagerBase? CurrentPlayList { get; protected set; }


    public override Task ChangeSongContainerAsync(ContainerBase? container, CancellationToken ctk = new())
    {
        CurrentSongContainer = container;
        return Task.CompletedTask;
    }

    public override async Task InsertSongAsync(SingleSongBase item, int index = -1, CancellationToken ctk = new())
    {
        await (CurrentPlayList?.AddSongAsync(item, index, ctk) ?? Task.CompletedTask);
    }

    public override async Task InsertSongRangeAsync(List<SingleSongBase> items, int index = -1, CancellationToken ctk = new())
    {
        await (CurrentPlayList?.AddSongRangeAsync(items, index, ctk) ?? Task.CompletedTask);
    }

    public override async Task RemoveSongAsync(SingleSongBase item, CancellationToken ctk = new())
    {
        await (CurrentPlayList?.RemoveSongAsync(item, ctk) ?? Task.CompletedTask);
    }

    public override async Task RemoveSongRangeAsync(List<SingleSongBase> items, CancellationToken ctk = new())
    {
        await (CurrentPlayList?.RemoveSongRangeAsync(items, ctk) ?? Task.CompletedTask);
    }

    public override async Task RemoveAllSongAsync(CancellationToken ctk = new())
    {
        await (CurrentPlayList?.ClearSongsAsync(ctk) ?? Task.CompletedTask);
    }

    public override async Task SetRandomAsync(bool isRandom, CancellationToken ctk = new())
    {
        IsRandom = isRandom;
        if (CurrentPlayListController is IRandomizablePlayListController randomizablePlayListController)
            await randomizablePlayListController.RandomizeAsync(isRandom ? DateTime.Now.Millisecond : -1, ctk)
                .ConfigureAwait(false);
    }

    public override async Task ReRandomAsync(CancellationToken ctk = new())
    {
        if (CurrentPlayListController is IRandomizablePlayListController randomizablePlayListController)
            await randomizablePlayListController.RandomizeAsync(DateTime.Now.Millisecond, ctk).ConfigureAwait(false);
    }

    public override async Task SetPlayModeAsync(string playModeId, CancellationToken ctk = new())
    {
        if (playModeId is not ("seq" or "sgl" or "shn" or "pfm" or "ltg"))
            return;

        ActivePlayModeId = playModeId;
        await SetRandomAsync(playModeId == "shn", ctk).ConfigureAwait(false);
    }

    public override Task<List<SingleSongBase>> GetPlaylistAsync(CancellationToken ctk = new())
    {
        return CurrentPlayList?.GetPlayListAsync(ctk) ?? Task.FromResult(new List<SingleSongBase>());
    }

    public override Task<List<SingleSongBase>> GetOrderedPlaylistAsync(CancellationToken ctk = new())
    {
        if (CurrentPlayListController is IPlayListGettablePlaylistController gettable)
            return gettable.GetOrderedPlayListAsync(ctk);

        return GetPlaylistAsync(ctk);
    }

    public override Task<int> GetCurrentIndexAsync(CancellationToken ctk = new())
    {
        if (CurrentPlayListController is IIndexedPlayListController indexed)
            return indexed.GetCurrentIndexAsync(ctk);

        return Task.FromResult(-1);
    }

    public override Task<SingleSongBase?> GetSongAtAsync(int index, CancellationToken ctk = new())
    {
        if (CurrentPlayListController is IIndexedPlayListController indexed)
            return indexed.GetSongAtAsync(index, ctk);

        return Task.FromResult<SingleSongBase?>(null);
    }

    public override async Task ReversePlaylistAsync(CancellationToken ctk = new())
    {
        if (CurrentPlayListController is IReversiblePlayListController reversible)
            await reversible.Reverse(ctk).ConfigureAwait(false);
    }
}
