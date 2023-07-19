namespace HyPlayer.PlayCore.Abstraction.Models.Lyric;

public class PreProcessedLyricLine
{
    public required long StartTime { get; set; }
    public required long EndTime { get; set; }
    public string? Text { get; set; }
}