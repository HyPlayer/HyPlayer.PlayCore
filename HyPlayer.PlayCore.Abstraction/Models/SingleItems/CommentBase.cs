using HyPlayer.PlayCore.Abstraction.Models.Containers;

namespace HyPlayer.PlayCore.Abstraction.Models.SingleItems;

public abstract class CommentBase : ProvidableItemBase
{
    public string? Content { get; set; }
    public long SendDate { get; set; }
    public DateTime SendTime => SendDate > 0 ? DateTimeOffset.FromUnixTimeMilliseconds(SendDate).LocalDateTime : DateTime.MinValue;
    public PersonBase? Sender { get; set; }
    public int LikedCount { get; set; }
    public int ReplyCount { get; set; }
    public bool HasLiked { get; set; }
    public bool IsMainComment { get; set; } = true;
    public string? ProvidableItemId { get; set; }
}
