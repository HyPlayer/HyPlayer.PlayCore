namespace HyPlayer.PlayCore.Abstraction.Models.Lyric;

public class PreProcessedLyricLine
{
    public required long StartTime { get; init; }
    public required long EndTime { get; init; }
    public string? Text { get; init; }
}