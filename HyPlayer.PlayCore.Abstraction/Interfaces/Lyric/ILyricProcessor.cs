using System.Collections.ObjectModel;
using HyPlayer.PlayCore.Abstraction.Models.Lyric;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.Lyric;

public interface ILyricProcessor
{
    public Task<ReadOnlyCollection<PreProcessedLyricLine>> ProcessLyric(string lrcText);
}