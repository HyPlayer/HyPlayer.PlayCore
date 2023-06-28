using HyPlayer.PlayCore.Abstraction.Models.Resources;

namespace HyPlayer.PlayCore.Abstraction.Models;

public abstract class ResourceBase
{
    public string? Url { get; init; }
    public string? ResourceName { get; init; }
    public bool HasContent { get; init; }
    public string? ExtensionName { get; init; }
    public abstract ResourceType Type { get; }
    public abstract Type ReturnType { get; }
    public abstract Task<object?> GetResource(ResourceQualityTag? qualityTag = null);
}

public enum ResourceType
{
    None,
    Image,
    Text,
    Video,
    Audio,
    Binary,
}