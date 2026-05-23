using HyPlayer.PlayCore.Abstraction.Interfaces.Cache;
using HyPlayer.PlayCore.Abstraction.Interfaces.Session;
using HyPlayer.PlayCore.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction.Models.Cache;
using HyPlayer.PlayCore.Abstraction.Models.Resources;

namespace HyPlayer.PlayCore.Tests;

public class PlayCoreFoundationTests
{
    [Test]
    public void Result_MatchesSuccessAndErrorBranches()
    {
        PlayCoreResult<int> success = 42;
        PlayCoreResult<int> error = new PlayCoreError("network", "Network unavailable");

        TestAssert.Ensure(success.IsSuccess && !success.IsError, "Success result should expose success state.");
        TestAssert.Ensure(success.Match(v => v + 1, _ => 0) == 43, "Match should invoke success branch.");
        TestAssert.Ensure(error.IsError && !error.IsSuccess, "Error result should expose error state.");
        TestAssert.Ensure(error.ErrorCode == "network" && error.ErrorMessage == "Network unavailable", "Error shape should expose code and message.");
        TestAssert.Ensure(error.Match(_ => "success", e => e.ErrorCode) == "network", "Match should invoke error branch.");
    }

    [Test]
    public void QualityTags_ExposeStableKeys()
    {
        var image = new ImageResourceQualityTag(300, 300);
        var music = MusicResourceQualityTag.Higher;

        TestAssert.Ensure(image.StableKey == "300y300" && image.ToString() == "300y300", "Image quality tag should keep existing stable string format.");
        TestAssert.Ensure(music.StableKey == "higher" && music.BitrateKbps == 320, "Music quality tag should expose provider-independent stable key.");
    }

    [Test]
    public void CacheKey_UsesProviderTypeActualKindAndQuality()
    {
        var key = ResourceCacheKey.FromQualityTag("ncm", "song", "123", ResourceKind.Audio, MusicResourceQualityTag.Lossless);

        TestAssert.Ensure(key.ProviderId == "ncm" && key.TypeId == "song" && key.ActualId == "123", "Cache key should preserve structured item identity.");
        TestAssert.Ensure(key.ResourceKind == ResourceKind.Audio && key.QualityStableKey == "lossless", "Cache key should preserve resource kind and quality key.");
        TestAssert.Ensure(key.ToString() == "ncm:song:123:audio:lossless", "Cache key string should be stable and structured.");
    }

    [Test]
    public async Task CacheProvider_GetOrCreateHonorsTtlVersionAndOfflineFallback()
    {
        var cache = new InMemoryCacheProvider();
        var key = new ResourceCacheKey("ncm", "song", "1", ResourceKind.Audio, "standard");
        var calls = 0;

        var first = await cache.GetOrCreateAsync(key, _ => Task.FromResult($"value-{++calls}"), new CacheEntryOptions { TimeToLive = TimeSpan.FromMinutes(1), Version = "v1" });
        var second = await cache.GetOrCreateAsync(key, _ => Task.FromResult($"value-{++calls}"), new CacheEntryOptions { TimeToLive = TimeSpan.FromMinutes(1), Version = "v1" });
        var changedVersion = await cache.GetOrCreateAsync(key, _ => Task.FromResult($"value-{++calls}"), new CacheEntryOptions { TimeToLive = TimeSpan.FromMinutes(1), Version = "v2" });

        TestAssert.Ensure(first.Value == "value-1" && second.Value == "value-1", "GetOrCreate should reuse valid cache entries.");
        TestAssert.Ensure(changedVersion.Value == "value-2", "Version changes should invalidate cached entries.");

        await cache.SetAsync(key, "stale", new CacheEntryOptions { TimeToLive = TimeSpan.FromMilliseconds(1), Version = "v3" });
        await Task.Delay(20);

        var withoutFallback = await cache.TryGetAsync<string>(key, new CacheEntryOptions { Version = "v3" });
        var withFallback = await cache.TryGetAsync<string>(key, new CacheEntryOptions { Version = "v3", AllowOfflineFallback = true });
        var factoryFallback = await cache.GetOrCreateAsync<string>(key, _ => throw new InvalidOperationException("offline"), new CacheEntryOptions { Version = "v3", AllowOfflineFallback = true });

        TestAssert.Ensure(withoutFallback.IsError, "Expired entries should miss when offline fallback is disabled.");
        TestAssert.Ensure(withFallback.IsSuccess && withFallback.Value == "stale", "Expired entries should be returned when offline fallback is enabled.");
        TestAssert.Ensure(factoryFallback.IsSuccess && factoryFallback.Value == "stale", "GetOrCreate should fall back to stale value when factory fails offline.");
    }

    [Test]
    public async Task SessionStore_IsolatesByProviderAndAccount()
    {
        var store = new InMemorySessionStore();

        await store.SetAsync("ncm", "account-a", new Dictionary<string, string> { ["cookie"] = "a" });
        await store.SetAsync("ncm", "account-b", new Dictionary<string, string> { ["cookie"] = "b" });
        await store.SetAsync("other", "account-a", new Dictionary<string, string> { ["cookie"] = "other" });

        var ncmA = await store.TryGetAsync("ncm", "account-a");
        var ncmB = await store.TryGetAsync("ncm", "account-b");
        var otherA = await store.TryGetAsync("other", "account-a");

        TestAssert.Ensure(ncmA.Value["cookie"] == "a", "Session should be scoped by provider and stable account key.");
        TestAssert.Ensure(ncmB.Value["cookie"] == "b", "Accounts under one provider should be isolated.");
        TestAssert.Ensure(otherA.Value["cookie"] == "other", "Same account key under another provider should be isolated.");

        await store.RemoveAsync("ncm", "account-a");
        TestAssert.Ensure((await store.TryGetAsync("ncm", "account-a")).IsError, "Removing one provider/account session should not remove others.");
        TestAssert.Ensure((await store.TryGetAsync("ncm", "account-b")).IsSuccess, "Removing one session should leave other accounts intact.");
    }

    private sealed class InMemoryCacheProvider : ICacheProvider
    {
        private readonly Dictionary<string, Entry> entries = new();

        public Task<PlayCoreResult<T>> TryGetAsync<T>(ResourceCacheKey key, CacheEntryOptions? options = null, CancellationToken ctk = default)
        {
            if (!entries.TryGetValue(key.ToString(), out var entry))
                return Task.FromResult(PlayCoreResult<T>.CreateError("cache.miss", "Cache entry was not found."));

            if (!IsUsable(entry, options) && options?.AllowOfflineFallback != true)
                return Task.FromResult(PlayCoreResult<T>.CreateError("cache.stale", "Cache entry is stale."));

            return entry.Value is T value
                ? Task.FromResult(PlayCoreResult<T>.CreateSuccess(value))
                : Task.FromResult(PlayCoreResult<T>.CreateError("cache.type_mismatch", "Cache entry type did not match."));
        }

        public Task SetAsync<T>(ResourceCacheKey key, T value, CacheEntryOptions? options = null, CancellationToken ctk = default)
        {
            entries[key.ToString()] = new Entry(value!, options?.Version, options?.TimeToLive == null ? null : DateTimeOffset.UtcNow.Add(options.TimeToLive.Value));
            return Task.CompletedTask;
        }

        public Task RemoveAsync(ResourceCacheKey key, CancellationToken ctk = default)
        {
            entries.Remove(key.ToString());
            return Task.CompletedTask;
        }

        public async Task<PlayCoreResult<T>> GetOrCreateAsync<T>(ResourceCacheKey key, Func<CancellationToken, Task<T>> factory, CacheEntryOptions? options = null, CancellationToken ctk = default)
        {
            var cached = await TryGetAsync<T>(key, options, ctk);
            if (cached.IsSuccess)
                return cached;

            try
            {
                var value = await factory(ctk);
                await SetAsync(key, value, options, ctk);
                return value;
            }
            catch (Exception ex)
            {
                if (options?.AllowOfflineFallback == true)
                {
                    var fallback = await TryGetAsync<T>(key, new CacheEntryOptions { AllowOfflineFallback = true, Version = options.Version }, ctk);
                    if (fallback.IsSuccess)
                        return fallback;
                }

                return PlayCoreResult<T>.CreateError("cache.factory_failed", ex.Message, ex);
            }
        }

        private static bool IsUsable(Entry entry, CacheEntryOptions? options)
        {
            return (options?.Version == null || options.Version == entry.Version) &&
                   (entry.ExpiresAt == null || entry.ExpiresAt > DateTimeOffset.UtcNow);
        }

        private sealed record Entry(object Value, string? Version, DateTimeOffset? ExpiresAt);
    }

    private sealed class InMemorySessionStore : ISessionStore
    {
        private readonly Dictionary<string, IReadOnlyDictionary<string, string>> sessions = new();

        public Task<PlayCoreResult<IReadOnlyDictionary<string, string>>> TryGetAsync(string providerId, string accountStableKey, CacheEntryOptions? options = null, CancellationToken ctk = default)
        {
            return sessions.TryGetValue(Key(providerId, accountStableKey), out var value)
                ? Task.FromResult(PlayCoreResult<IReadOnlyDictionary<string, string>>.CreateSuccess(value))
                : Task.FromResult(PlayCoreResult<IReadOnlyDictionary<string, string>>.CreateError("session.miss", "Session was not found."));
        }

        public Task SetAsync(string providerId, string accountStableKey, IReadOnlyDictionary<string, string> values, CacheEntryOptions? options = null, CancellationToken ctk = default)
        {
            sessions[Key(providerId, accountStableKey)] = new Dictionary<string, string>(values);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string providerId, string accountStableKey, CancellationToken ctk = default)
        {
            sessions.Remove(Key(providerId, accountStableKey));
            return Task.CompletedTask;
        }

        private static string Key(string providerId, string accountStableKey) => providerId + ":" + accountStableKey;
    }
}
