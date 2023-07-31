using HyPlayer.PlayCore.Abstraction.Models;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

public interface ICommentProvidable
{
    public Task<ContainerBase?> GetCommentContainer();
}