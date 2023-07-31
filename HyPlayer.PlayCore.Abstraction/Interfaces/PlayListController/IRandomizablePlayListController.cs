﻿using System.Collections.ObjectModel;
using HyPlayer.PlayCore.Abstraction.Models.SingleItems;

namespace HyPlayer.PlayCore.Abstraction.Interfaces.PlayListController;

public interface IRandomizablePlayListController
{
    public Task Randomize(int seed = -1);
    public Task<List<SingleSongBase>> GetOriginalList();
}