namespace HyPlayer.PlayCore.Abstraction.Models.Lyric;

public class CompoundedLyricLine
{
    public required long StartTime { get; set; }
    public required long EndTime { get; set; }
    public string? OriginalText { get; set; }
}