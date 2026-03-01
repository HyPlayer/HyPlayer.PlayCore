using HyPlayer.PlayCore.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;

namespace HyPlayer.PlayCore
{
    public partial class Chopin
    {
        public override Task RegisterAudioServiceAsync(Type serviceType, CancellationToken ctk = default)
        {
            throw new NotImplementedException();
        }

        public override Task RegisterMusicProviderAsync(Type serviceType, CancellationToken ctk = default)
        {
            throw new NotImplementedException();
        }

        public override Task RegisterPlayListControllerAsync(Type serviceType, CancellationToken ctk = default)
        {
            throw new NotImplementedException();
        }

        public override Task FocusAudioServiceAsync(Type serviceType, CancellationToken ctk = default)
        {
            throw new NotImplementedException();
        }

        public override Task FocusPlayListControllerAsync(Type serviceType, CancellationToken ctk = default)
        {
            throw new NotImplementedException();
        }


        public override Task UnregisterAudioServiceAsync(Type serviceType, CancellationToken ctk = default)
        {
            throw new NotImplementedException();
        }

        public override Task UnregisterMusicProviderAsync(Type serviceType, CancellationToken ctk = default)
        {
            throw new NotImplementedException();
        }

        public override Task UnregisterPlayListControllerAsync(Type serviceType, CancellationToken ctk = default)
        {
            throw new NotImplementedException();
        }
    }
}
