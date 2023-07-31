using HyPlayer.PlayCore.Abstraction.Models.Resources;
using HyPlayer.PlayCore.Abstraction.Models.SingleItems;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

public interface IMusicResourceProvidable
{
    public Task<MusicResourceBase?> GetMusicResource(SingleSongBase song, ResourceQualityTag qualityTag);
}