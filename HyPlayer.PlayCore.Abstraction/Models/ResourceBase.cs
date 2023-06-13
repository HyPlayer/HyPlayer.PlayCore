using HyPlayer.PlayCore.Abstraction.Models.Resources;

namespace HyPlayer.PlayCore.Abstraction.Models;

public abstract class ResourceBase
{
    public string? Url { get; }
    public string? ResourceName { get; }
    public bool HasContent { get; }
    public string? ExtensionName { get; }
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