using HyPlayer.PlayCore.Abstraction.Models.SingleItems;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.PlayListController;

public interface IOrderedRandomPlayListController : IRandomizablePlayListController
{
    public Task ApplyRandomOrderAsync(IReadOnlyList<SingleSongBase> order, CancellationToken ctk = new());
}
