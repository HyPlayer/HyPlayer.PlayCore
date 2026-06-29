namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

public interface IProviderKnownTypeIds : IProvider
{
    string SingleSongTypeId { get; }
    string PlaylistTypeId { get; }
    string ArtistTypeId { get; }
    string AlbumTypeId { get; }
    string UserTypeId { get; }
    string? RadioChannelTypeId { get; }
    string? RichMediaTypeId { get; }
}
