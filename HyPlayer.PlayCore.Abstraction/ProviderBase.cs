namespace HyPlayer.PlayCore.Abstraction;

public abstract class ProviderBase
{
    public abstract string Name { get; }
    public abstract string Id { get; }
    public abstract List<ProvidableTypeId> ProvidableTypeIds { get; }
}

public record ProvidableTypeId(string Id, string Name, bool Observable);