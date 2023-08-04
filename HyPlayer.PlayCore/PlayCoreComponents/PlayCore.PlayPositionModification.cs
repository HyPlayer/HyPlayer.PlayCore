using HyPlayer.PlayCore.Abstraction;
using HyPlayer.PlayCore.Abstraction.Interfaces.PlayListController;
using HyPlayer.PlayCore.Abstraction.Models.SingleItems;

namespace HyPlayer.PlayCore;

public sealed partial class Chopin
{
    public override async Task MovePointerToAsync(SingleSongBase song)
    {
        if (CurrentPlayListController is INavigateSongPlayListController controller)
            await controller.NavigateSongTo(song);
    }

    public override async Task MoveNextAsync()
    {
        if (CurrentPlayListController is { } controller)
            await controller.MoveNextAsync();
    }

    public override async Task MovePreviousAsync()
    {
        if (CurrentPlayListController is { } controller)
            await controller.MovePreviousAsync();
    }
}