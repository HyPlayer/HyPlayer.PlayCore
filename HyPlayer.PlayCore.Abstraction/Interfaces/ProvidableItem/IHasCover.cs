using HyPlayer.PlayCore.Abstraction.Models.Resources;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.ProvidableItem;

public interface IHasCover
{
    public Task<ImageResourceBase?> GetCover();
}