using HyPlayer.PlayCore.Abstraction.Models.Resources;
using HyPlayer.PlayCore.Abstraction.Models.SingleItems;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Playback;

public interface IPlaybackQualitySelector
{
    ResourceQualityTag? GetPlaybackQualityTag(SingleSongBase song);
}
