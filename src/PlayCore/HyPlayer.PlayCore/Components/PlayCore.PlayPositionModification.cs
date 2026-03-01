using HyPlayer.PlayCore.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction.Models.SingleItems;
using System;
using System.Collections.Generic;
using System.Text;

namespace HyPlayer.PlayCore
{
    public partial class Chopin
    {
        public override Task ChangeSongContainerAsync(ContainerBase? container, CancellationToken ctk = default)
        {
            throw new NotImplementedException();
        }


        public override Task InsertSongAsync(SingleSongBase item, int index = -1, CancellationToken ctk = default)
        {
            throw new NotImplementedException();
        }

        public override Task InsertSongRangeAsync(List<SingleSongBase> items, int index = -1, CancellationToken ctk = default)
        {
            throw new NotImplementedException();
        }

        public override Task RemoveAllSongAsync(CancellationToken ctk = default)
        {
            throw new NotImplementedException();
        }

        public override Task RemoveSongAsync(SingleSongBase item, CancellationToken ctk = default)
        {
            throw new NotImplementedException();
        }

        public override Task RemoveSongRangeAsync(List<SingleSongBase> item, CancellationToken ctk = default)
        {
            throw new NotImplementedException();
        }

        public override Task ReRandomAsync(CancellationToken ctk = default)
        {
            throw new NotImplementedException();
        }

        public override Task SetRandomAsync(bool isRandom, CancellationToken ctk = default)
        {
            throw new NotImplementedException();
        }
    }
}
