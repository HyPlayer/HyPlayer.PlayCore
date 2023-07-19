using HyPlayer.PlayCore.Abstraction.Models.Resources;

namespace HyPlayer.PlayCore.Abstraction.Models;

public abstract class ResourceBase
{
    public string? Url { get; set; }
    public string? ResourceName { get; set; }
    public bool HasContent { get; set; }
    public string? ExtensionName { get; set; }
    public abstract ResourceType Type { get; }
    public abstract Task<object?> GetResource(ResourceQualityTag? qualityTag = null, Type? awaitingType = null);
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