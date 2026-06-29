using HyPlayer.PlayCore.Abstraction.Interfaces.PlayListController;
using HyPlayer.PlayCore.Abstraction.Models.SingleItems;

namespace HyPlayer.PlayCore;

public sealed partial class Chopin
{
    public override async Task MovePointerToAsync(SingleSongBase song, CancellationToken ctk = new())
    {
        if (CurrentPlayListController is INavigateSongPlayListController controller)
            CurrentSong = await controller.NavigateSongToAsync(song, ctk).ConfigureAwait(false) ?? CurrentSong;
    }

    public override async Task MovePointerToIndexAsync(int index, CancellationToken ctk = new())
    {
        if (CurrentPlayListController is IIndexedPlayListController indexedController)
            CurrentSong = await indexedController.MoveToIndexAsync(index, ctk).ConfigureAwait(false) ?? CurrentSong;
    }

    public override async Task MoveNextAsync(CancellationToken ctk = new())
    {
        if (CurrentPlayListController is { } controller)
            CurrentSong = await controller.MoveNextAsync(ctk).ConfigureAwait(false);
    }

    public override async Task MovePreviousAsync(CancellationToken ctk = new())
    {
        if (CurrentPlayListController is { } controller)
            CurrentSong = await controller.MovePreviousAsync(ctk).ConfigureAwait(false);
    }
}
