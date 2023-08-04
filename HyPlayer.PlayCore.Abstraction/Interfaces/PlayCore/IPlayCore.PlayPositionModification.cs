using HyPlayer.PlayCore.Abstraction.Models.SingleItems;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.PlayCore;

public interface IPlayCorePlayPositionModification
{
    public Task MovePointerToAsync(SingleSongBase song);
    public Task MoveNextAsync();
    public Task MovePreviousAsync();
}