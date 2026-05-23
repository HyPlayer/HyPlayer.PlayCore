namespace HyPlayer.PlayCore.Abstraction.Models;

/// <summary>
/// Describes a provider-neutral category that can be used to query or group containers.
/// </summary>
public sealed class ProviderCategory
{
    /// <summary>
    /// Gets or sets the provider-scoped category id.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Gets or sets the display name of the category.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the optional parent category id.
    /// </summary>
    public string? ParentId { get; set; }
}
