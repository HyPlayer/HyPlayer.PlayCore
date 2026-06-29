namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

/// <summary>
/// Provides provider-defined type ids for common search categories.
/// </summary>
public interface IProviderSearchCategoryTypeIds : IProvider
{
    string SingleSongSearchTypeId { get; }
    string AlbumSearchTypeId { get; }
    string ArtistSearchTypeId { get; }
    string PlaylistSearchTypeId { get; }
    string UserSearchTypeId { get; }
    string? RadioChannelSearchTypeId { get; }
    string? RichMediaSearchTypeId { get; }
    string? ShortVideoSearchTypeId { get; }
    string? LyricSearchTypeId { get; }
}
