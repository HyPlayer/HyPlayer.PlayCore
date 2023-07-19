namespace HyPlayer.PlayCore.Abstraction.Models;

public abstract class ProvidableItemBase
{
    public required string Name { get; set; }
    public string Id => ProviderId + TypeId + ActualId;
    public abstract string ProviderId { get; set; }
    public abstract string TypeId { get; set; }
    public required string ActualId { get; set; }
}