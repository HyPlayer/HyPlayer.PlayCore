namespace HyPlayer.PlayCore.Abstraction.Models.Resources;

public class MusicResourceQualityTag : ResourceQualityTag
{
    public MusicResourceQualityTag(string stableKey, int? bitrateKbps = null)
    {
        StableKey = stableKey;
        BitrateKbps = bitrateKbps;
    }

    public static MusicResourceQualityTag Standard { get; } = new MusicResourceQualityTag("standard", 128);
    public static MusicResourceQualityTag Higher { get; } = new MusicResourceQualityTag("higher", 320);
    public static MusicResourceQualityTag Lossless { get; } = new MusicResourceQualityTag("lossless");
    public static MusicResourceQualityTag HiRes { get; } = new MusicResourceQualityTag("hires");

    public override string StableKey { get; }
    public int? BitrateKbps { get; }

    public override string ToString()
    {
        return StableKey;
    }
}
