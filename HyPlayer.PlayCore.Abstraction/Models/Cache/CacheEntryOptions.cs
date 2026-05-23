using System;

namespace HyPlayer.PlayCore.Abstraction.Models.Cache;

public sealed class CacheEntryOptions
{
    public TimeSpan? TimeToLive { get; set; }
    public string? Version { get; set; }
    public bool AllowOfflineFallback { get; set; }
}
