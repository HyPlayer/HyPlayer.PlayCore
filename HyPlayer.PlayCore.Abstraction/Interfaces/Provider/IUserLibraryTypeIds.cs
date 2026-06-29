namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

/// <summary>
/// Provides provider-defined ids for common user library containers.
/// </summary>
public interface IUserLibraryTypeIds : IProvider
{
    string CloudLibraryTypeId { get; }
    string LikedSongsTypeId { get; }
    string RecentListeningHistoryTypeId { get; }
    string AllListeningHistoryTypeId { get; }
}
