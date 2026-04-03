using HyPlayer.PlayCore.Abstraction;
using HyPlayer.PlayCore.Abstraction.Interfaces.Wrapper;
using HyPlayer.PlayCore.Wrapper;

namespace HyPlayer.PlayCore
{
    public partial class Chopin
    {
        private readonly IPlayCoreWrapper _wrapper;

        public override Task RegisterAudioServiceAsync(Type serviceType, CancellationToken ctk = default)
        {
            if (_wrapper is not null)
            {
                _wrapper.AddAudioService(serviceType);
            }
            return Task.CompletedTask;
        }

        public override Task RegisterMusicProviderAsync(Type serviceType, CancellationToken ctk = default)
        {
            if (_wrapper is not null)
            {
                _wrapper.AddProvider(serviceType);
            }
            return Task.CompletedTask;
        }

        public override Task RegisterPlayControllerAsync(Type serviceType, CancellationToken ctk = default)
        {
            if (_wrapper is not null)
            {
                _wrapper.AddPlayController(serviceType);
            }
            return Task.CompletedTask;
        }

        public override Task FocusAudioServiceAsync(Type serviceType, CancellationToken ctk = default)
        {
            if (_wrapper is not null)
            {
                _wrapper.SetCurrentAudioService(serviceType);
            }
            return Task.CompletedTask;
        }

        public override Task FocusPlayListControllerAsync(Type serviceType, CancellationToken ctk = default)
        {
             if (_wrapper is not null)
            {
                _wrapper.SetCurrentPlayistController(serviceType);
            }
            return Task.CompletedTask;
        }


        public override Task UnregisterAudioServiceAsync(Type serviceType, CancellationToken ctk = default)
        {
            if (_wrapper is not null)
            {
                _wrapper.RemoveAudioService(serviceType);
            }
            return Task.CompletedTask;
        }

        public override Task UnregisterMusicProviderAsync(Type serviceType, CancellationToken ctk = default)
        {
            if (_wrapper is not null)
            {
                _wrapper.RemoveProvider(serviceType);
            }
            return Task.CompletedTask;
        }

        public override Task UnregisterPlayControllerAsync(Type serviceType, CancellationToken ctk = default)
        {
            if (_wrapper is not null)
            {
                _wrapper.RemovePlayController(serviceType);
            }
            return Task.CompletedTask;
        }
    }
}
