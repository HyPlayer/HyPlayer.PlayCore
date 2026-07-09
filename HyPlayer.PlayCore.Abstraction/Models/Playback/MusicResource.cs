namespace HyPlayer.Domain.Music;

public abstract record MusicResource
{
    private MusicResource()
    {
    }

    public sealed record Album(string Id) : MusicResource;
    public sealed record Artist(string Id) : MusicResource;
    public sealed record DailyRecommend(string Id) : MusicResource;
    public sealed record Playlist(string Id) : MusicResource;
    public sealed record Radio(string Id) : MusicResource;
    public sealed record Song(string Id) : MusicResource;

    public string ToPlaybackSourceKey() => this switch
    {
        Album album => "al" + album.Id,
        Artist artist => "ar" + artist.Id,
        DailyRecommend dailyRecommend => "dr" + dailyRecommend.Id,
        Playlist playlist => "pl" + playlist.Id,
        Radio radio => "rd" + radio.Id,
        Song song => "ns" + song.Id,
        _ => throw new System.InvalidOperationException("Unsupported music resource type.")
    };

    public static bool TryParseExternalResource(string value, out MusicResource resource)
    {
        var id = value.Length >= 2 ? value.Substring(2) : string.Empty;
        if (value.Length >= 2 && value.StartsWith("dr") && id.Length > 0)
        {
            resource = new DailyRecommend(id);
            return true;
        }

        resource = value.Length >= 2 && IsNumeric(id) ? value.Substring(0, 2) switch
        {
            "al" => new Album(id),
            "ar" => new Artist(id),
            "ns" => new Song(id),
            "pl" => new Playlist(id),
            "rd" => new Radio(id),
            _ => null!
        } : null!;

        return resource is not null;
    }

    private static bool IsNumeric(string value)
    {
        if (value.Length == 0) return false;
        foreach (var c in value)
            if (!char.IsDigit(c)) return false;
        return true;
    }
}
