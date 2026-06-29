namespace HyPlayer.PlayCore.Abstraction.Models.Lyric;

public abstract class RawLyricInfo : ResourceBase
{
    public override ResourceType Type => ResourceType.Text;
    public abstract LyricType LyricType { get; }
    public string? Source { get; set; }
}

public sealed class TextResourceResult : ResourceResultBase, IResourceResultOf<string>
{
    public override Exception? ExternalException { get; init; }
    public override required ResourceStatus ResourceStatus { get; init; }
    public string? Content { get; init; }

    public Task<string?> GetResourceAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Content);
    }
}

public enum LyricType
{
    Original,
    Translation,
    Romaji,
    Furigana,
    Commentary,
    Soramimi, // 空耳
}
