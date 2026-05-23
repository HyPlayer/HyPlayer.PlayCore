using System;
using System.Threading;
using System.Threading.Tasks;
using HyPlayer.PlayCore.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction.Models.Cache;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Cache;

public interface ICacheProvider
{
    Task<PlayCoreResult<T>> TryGetAsync<T>(ResourceCacheKey key, CacheEntryOptions? options = null, CancellationToken ctk = default);

    Task SetAsync<T>(ResourceCacheKey key, T value, CacheEntryOptions? options = null, CancellationToken ctk = default);

    Task RemoveAsync(ResourceCacheKey key, CancellationToken ctk = default);

    Task<PlayCoreResult<T>> GetOrCreateAsync<T>(ResourceCacheKey key, Func<CancellationToken, Task<T>> factory, CacheEntryOptions? options = null, CancellationToken ctk = default);
}
