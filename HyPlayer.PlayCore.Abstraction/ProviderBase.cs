namespace HyPlayer.PlayCore.Abstraction;

public abstract class ProviderBase
{
    public required string Name { get; init; }
    public required string Id { get; init; }
}