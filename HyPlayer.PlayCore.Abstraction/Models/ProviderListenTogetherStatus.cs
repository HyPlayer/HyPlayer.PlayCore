namespace HyPlayer.PlayCore.Abstraction.Models;

public sealed class ProviderListenTogetherStatus
{
    public bool IsInRoom { get; init; }
    public string? RoomId { get; init; }
    public List<ProviderListenTogetherUser> Users { get; init; } = [];
}

public sealed class ProviderListenTogetherUser
{
    public required string UserId { get; init; }
    public required string Nickname { get; init; }
    public required string AvatarUrl { get; init; }
}
