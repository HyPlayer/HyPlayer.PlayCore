using HyPlayer.PlayCore.Abstraction.Models.Lyric;
using HyPlayer.PlayCore.Abstraction.Models.Songs;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Provider;

public interface ILyricProvidable
{
    public Task<List<RawLyricInfo>> GetLyricInfo(SingleSongBase song);
}