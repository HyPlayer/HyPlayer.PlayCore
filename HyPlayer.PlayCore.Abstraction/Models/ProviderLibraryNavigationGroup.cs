using HyPlayer.PlayCore.Abstraction.Models.Containers;

namespace HyPlayer.PlayCore.Abstraction.Models;

public sealed class ProviderLibraryNavigationGroup
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public IReadOnlyList<ContainerBase> Items { get; init; } = [];
    public int DisplayOrder { get; init; }
    public bool IsPinned { get; init; }
}
