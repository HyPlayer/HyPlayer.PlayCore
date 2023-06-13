﻿namespace HyPlayer.PlayCore.Abstraction.Models;

public abstract class ProvidableItemBase
{
    public required string Name { get; init; }
    public string Id => ProviderId + TypeId + ActualId;
    public abstract string ProviderId { get; }
    public abstract string TypeId { get; }
    public required string ActualId { get; init; }
}