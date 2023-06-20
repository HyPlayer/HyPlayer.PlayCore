namespace HyPlayer.PlayCore.Abstraction.Models.Lyric;

public class CompoundedLyricLine
{
    public required long StartTime { get; init; }
    public required long EndTime { get; init; }
    public string? OriginalText { get; init; }
}