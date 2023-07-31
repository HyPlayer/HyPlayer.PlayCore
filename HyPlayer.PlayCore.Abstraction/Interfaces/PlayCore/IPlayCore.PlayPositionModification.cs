using HyPlayer.PlayCore.Abstraction.Models.SingleItems;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.PlayCore;

public interface IPlayCorePlayPositionModification
{
    public Task MovePointerTo(SingleSongBase song);
    public Task MoveNext();
    public Task MovePrevious();
}