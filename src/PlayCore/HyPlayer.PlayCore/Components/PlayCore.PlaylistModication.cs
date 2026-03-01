using HyPlayer.PlayCore.Abstraction;
using HyPlayer.PlayCore.Abstraction.Models.SingleItems;
using System;
using System.Collections.Generic;
using System.Text;

namespace HyPlayer.PlayCore
{
    public partial class Chopin : PlayCoreBase
    {
        public override Task MoveNextAsync(CancellationToken ctk = default)
        {
            throw new NotImplementedException();
        }

        public override Task MovePointerToAsync(SingleSongBase song, CancellationToken ctk = default)
        {
            throw new NotImplementedException();
        }

        public override Task MovePreviousAsync(CancellationToken ctk = default)
        {
            throw new NotImplementedException();
        }
    }
}
