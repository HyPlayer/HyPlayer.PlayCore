using HyPlayer.PlayCore.Abstraction.Models;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

/// <summary>
/// Provides QR-code based provider authentication.
/// </summary>
public interface IQrAuthenticationProvidable : IProvider
{
    /// <summary>
    /// Creates a new provider QR login challenge.
    /// </summary>
    /// <param name="ctk">The cancellation token for the operation.</param>
    /// <returns>The created QR login challenge.</returns>
    public Task<ProviderQrLoginChallenge> CreateQrLoginChallengeAsync(CancellationToken ctk = new());

    /// <summary>
    /// Polls the state of a provider QR login challenge.
    /// </summary>
    /// <param name="challengeId">The challenge id returned by <see cref="CreateQrLoginChallengeAsync" />.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    /// <returns>The current QR login state.</returns>
    public Task<ProviderQrLoginState> GetQrLoginStateAsync(string challengeId, CancellationToken ctk = new());
}
