namespace HyPlayer.PlayCore.Abstraction;

public abstract class ProviderBase
{
    public abstract string Name { get; }
    public abstract string Id { get; }
    public abstract Dictionary<string, string> TypeIdToNameDictionary { get; }
}