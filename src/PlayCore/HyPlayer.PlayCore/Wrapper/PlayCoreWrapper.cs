using HyPlayer.PlayCore.Abstraction;
using HyPlayer.PlayCore.Abstraction.Interfaces.Wrapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace HyPlayer.PlayCore.Wrapper
{
    public class PlayCoreWrapper : IPlayCoreWrapper
    {
        private readonly HashSet<AudioServiceBase> _audioServices = new();
        private readonly HashSet<ProviderBase> _musicProviders = new();
        private readonly HashSet<PlayListManagerBase> _playlistControllers = new();

        public ObservableCollection<AudioServiceBase> AudioServices 
            { get => new( new ObservableCollection<AudioServiceBase>(_audioServices.ToList())); }
        public ObservableCollection<ProviderBase> MusicProviders
            { get => new(new ObservableCollection<ProviderBase>(_musicProviders.ToList())); }
        public ObservableCollection<PlayListManagerBase> PlayListControllers
            { get => new(new ObservableCollection<PlayListManagerBase>(_playlistControllers.ToList())); }


        public PlayCoreWrapper()
        {
            
        }

        public void AddAudioService(Type serviceType)
        {
            _audioServices.RemoveWhere(t => t.GetType() == serviceType);
            _audioServices.Add((AudioServiceBase) Activator.CreateInstance(serviceType));
        }

        public void AddProvider(Type providerType)
        {
            _musicProviders.RemoveWhere(t => t.GetType() == providerType);
            _musicProviders.Add((ProviderBase) Activator.CreateInstance(providerType));
        }

        public void AddPlayListManager(Type managerType)
        {
            _playlistControllers.RemoveWhere(t => t.GetType() == managerType);
            _playlistControllers.Add((PlayListManagerBase) Activator.CreateInstance(managerType));
        }

        public void RemoveAudioService(Type serviceType)
        {
            _audioServices.Remove((AudioServiceBase)Activator.CreateInstance(serviceType));
        }

        public void RemoveProvider(Type providerType)
        {
            _musicProviders.Remove((ProviderBase)Activator.CreateInstance(providerType));
        }

        public void RemovePlayListManager(Type managerType)
        {
            _playlistControllers.Remove((PlayListManagerBase)Activator.CreateInstance(managerType));
        }
    }
}
