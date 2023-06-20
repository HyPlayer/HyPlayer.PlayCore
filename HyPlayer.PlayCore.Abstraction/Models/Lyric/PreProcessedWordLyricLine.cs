using System.Collections.ObjectModel;

namespace HyPlayer.PlayCore.Abstraction.Models.Lyric;

public class PreProcessedWordLyricLine : PreProcessedLyricLine
{
    public required ReadOnlyCollection<LyricWord> Words { get; init; }
}