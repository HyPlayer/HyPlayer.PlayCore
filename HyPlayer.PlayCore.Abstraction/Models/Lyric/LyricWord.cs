namespace HyPlayer.PlayCore.Abstraction.Models.Lyric;

public class LyricWord
{
    public string? Word { get; init; }
    public required long StartTime { get; init; }
    public required long EndTime { get; init; }
}