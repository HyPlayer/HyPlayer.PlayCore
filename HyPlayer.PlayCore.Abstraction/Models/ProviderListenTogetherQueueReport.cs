using HyPlayer.PlayCore.Abstraction.Models.SingleItems;

namespace HyPlayer.PlayCore.Abstraction.Models;

public sealed class ProviderListenTogetherQueueReport
{
    public required List<SingleSongBase> Queue { get; init; }
    public string? UserId { get; init; }
    public string PlayModeId { get; init; } = "seq";
    public int ClientSeq { get; init; }
    public int AnchorPosition { get; init; } = -1;
    public string? AnchorItemId { get; init; }
}
