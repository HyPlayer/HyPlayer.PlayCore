using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HyPlayer.PlayCore.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction.Models.Cache;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Session;

public interface ISessionStore
{
    Task<PlayCoreResult<IReadOnlyDictionary<string, string>>> TryGetAsync(string providerId, string accountStableKey, CacheEntryOptions? options = null, CancellationToken ctk = default);

    Task SetAsync(string providerId, string accountStableKey, IReadOnlyDictionary<string, string> values, CacheEntryOptions? options = null, CancellationToken ctk = default);

    Task RemoveAsync(string providerId, string accountStableKey, CancellationToken ctk = default);
}
