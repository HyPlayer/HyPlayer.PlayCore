namespace HyPlayer.PlayCore.Abstraction.Models.Resources;

/// <summary>
///     Provides per-track loudness metadata for audio gain normalization.
/// </summary>
public interface IAudioGainMetadata
{
    /// <summary>
    ///     Gain in decibels relative to the provider's -18 dB reference loudness.
    /// </summary>
    double? GainDb { get; }

    /// <summary>
    ///     Linear audio peak relative to full scale.
    /// </summary>
    double? Peak { get; }

    /// <summary>
    ///     Whether this resource is a stereo source suitable for gain normalization.
    /// </summary>
    bool SupportsAudioGain { get; }
}
