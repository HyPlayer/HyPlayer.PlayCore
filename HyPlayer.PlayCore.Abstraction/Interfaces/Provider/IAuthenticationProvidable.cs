using HyPlayer.PlayCore.Abstraction.Models;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

/// <summary>
/// Provides provider-owned authentication and session management operations.
/// </summary>
public interface IAuthenticationProvidable : IProvider
{
    /// <summary>
    /// Attempts to authenticate the provider session with a username-like identifier and secret.
    /// </summary>
    /// <param name="accountId">The provider-neutral account identifier, such as an email address or phone number.</param>
    /// <param name="secret">The authentication secret supplied by the caller.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    /// <returns>The authenticated session state produced by the provider.</returns>
    public Task<ProviderSessionInfo> LoginAsync(string accountId, string secret, CancellationToken ctk = new());

    /// <summary>
    /// Ends the provider-owned session and clears provider-side credentials when possible.
    /// </summary>
    /// <param name="ctk">The cancellation token for the operation.</param>
    public Task LogoutAsync(CancellationToken ctk = new());

    /// <summary>
    /// Gets the current provider-owned session state without exposing provider-specific storage.
    /// </summary>
    /// <param name="ctk">The cancellation token for the operation.</param>
    /// <returns>The current provider session state.</returns>
    public Task<ProviderSessionInfo> GetSessionInfoAsync(CancellationToken ctk = new());

    /// <summary>
    /// Replaces provider-owned session values with serialized values supplied by the app shell.
    /// </summary>
    /// <param name="sessionValues">Provider-neutral key/value session data previously exported by the provider.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    public Task ImportSessionAsync(IReadOnlyDictionary<string, string> sessionValues, CancellationToken ctk = new());

    /// <summary>
    /// Exports provider-owned session values for app-side persistence without exposing provider APIs.
    /// </summary>
    /// <param name="ctk">The cancellation token for the operation.</param>
    /// <returns>Provider-neutral key/value session data.</returns>
    public Task<IReadOnlyDictionary<string, string>> ExportSessionAsync(CancellationToken ctk = new());

    /// <summary>
    /// Announces the current device to the provider session when the provider requires device binding.
    /// </summary>
    /// <param name="deviceName">The human-readable device name to announce.</param>
    /// <param name="ctk">The cancellation token for the operation.</param>
    public Task AnnounceDeviceAsync(string deviceName, CancellationToken ctk = new());
}
