namespace HyPlayer.PlayCore.Abstraction.Models;

/// <summary>
/// Describes a provider-issued QR authentication challenge.
/// </summary>
public sealed class ProviderQrLoginChallenge
{
    /// <summary>
    /// Gets or sets the provider-neutral challenge id used for subsequent polling.
    /// </summary>
    public required string ChallengeId { get; set; }

    /// <summary>
    /// Gets or sets the URI that should be rendered or encoded for QR authentication.
    /// </summary>
    public required Uri Uri { get; set; }

    /// <summary>
    /// Gets or sets the point in time when the challenge expires, when the provider exposes one.
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; set; }
}

/// <summary>
/// Describes the provider-neutral state of a QR authentication challenge.
/// </summary>
public sealed class ProviderQrLoginState
{
    /// <summary>
    /// Gets or sets the current QR login status.
    /// </summary>
    public ProviderQrLoginStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the session information when QR login has completed.
    /// </summary>
    public ProviderSessionInfo? SessionInfo { get; set; }

    /// <summary>
    /// Gets or sets a provider-neutral message suitable for logs or diagnostics.
    /// </summary>
    public string? Message { get; set; }
}

/// <summary>
/// Identifies provider-neutral QR authentication states.
/// </summary>
public enum ProviderQrLoginStatus
{
    /// <summary>
    /// The challenge has been created but not scanned.
    /// </summary>
    WaitingForScan,

    /// <summary>
    /// The challenge has been scanned and is waiting for user confirmation.
    /// </summary>
    WaitingForConfirmation,

    /// <summary>
    /// The challenge completed and produced a session.
    /// </summary>
    Authorized,

    /// <summary>
    /// The challenge expired before completion.
    /// </summary>
    Expired,

    /// <summary>
    /// The challenge was rejected or failed.
    /// </summary>
    Failed
}
