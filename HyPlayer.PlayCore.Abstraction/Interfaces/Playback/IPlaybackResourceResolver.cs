using HyPlayer.PlayCore.Abstraction.Interfaces.Provider;
using HyPlayer.PlayCore.Abstraction.Models.Resources;
using HyPlayer.PlayCore.Abstraction.Models.SingleItems;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Playback;

public interface IPlaybackResourceResolver
{
    Task<MusicResourceBase?> ResolvePlaybackResourceAsync(
        SingleSongBase song,
        IMusicResourceProvidable provider,
        ResourceQualityTag? qualityTag,
        CancellationToken ctk = default);
}
