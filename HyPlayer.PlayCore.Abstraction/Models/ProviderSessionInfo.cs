namespace HyPlayer.PlayCore.Abstraction.Models;

/// <summary>
/// Describes provider-owned authentication state without exposing provider-specific session objects.
/// </summary>
public sealed class ProviderSessionInfo
{
    /// <summary>
    /// Gets or sets whether the provider considers the current session authenticated.
    /// </summary>
    public bool IsAuthenticated { get; set; }

    /// <summary>
    /// Gets or sets the provider-scoped user id associated with the session, when one is available.
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Gets or sets the provider-scoped display name associated with the session, when one is available.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the point in time when the provider session expires, when the provider exposes one.
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; set; }
}
