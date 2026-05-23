using System;
using HyPlayer.PlayCore.Abstraction.Models.Resources;

namespace HyPlayer.PlayCore.Abstraction.Models.Cache;

public sealed class ResourceCacheKey : IEquatable<ResourceCacheKey>
{
    public ResourceCacheKey(string providerId, string typeId, string actualId, ResourceKind resourceKind, string? qualityStableKey = null)
    {
        ProviderId = providerId;
        TypeId = typeId;
        ActualId = actualId;
        ResourceKind = resourceKind;
        QualityStableKey = string.IsNullOrEmpty(qualityStableKey) ? "default" : qualityStableKey!;
    }

    public string ProviderId { get; }
    public string TypeId { get; }
    public string ActualId { get; }
    public ResourceKind ResourceKind { get; }
    public string QualityStableKey { get; }

    public static ResourceCacheKey FromQualityTag(string providerId, string typeId, string actualId, ResourceKind resourceKind, ResourceQualityTag? qualityTag)
    {
        return new ResourceCacheKey(providerId, typeId, actualId, resourceKind, qualityTag?.StableKey);
    }

    public override string ToString()
    {
        return string.Join(":", new[]
        {
            Escape(ProviderId),
            Escape(TypeId),
            Escape(ActualId),
            ResourceKind.ToString().ToLowerInvariant(),
            Escape(QualityStableKey)
        });
    }

    public bool Equals(ResourceCacheKey? other)
    {
        return other != null && ToString() == other.ToString();
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as ResourceCacheKey);
    }

    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }

    private static string Escape(string value)
    {
        return Uri.EscapeDataString(value);
    }
}
