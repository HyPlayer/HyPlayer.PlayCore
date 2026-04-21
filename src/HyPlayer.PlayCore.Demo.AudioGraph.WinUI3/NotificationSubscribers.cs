using CommunityToolkit.Mvvm.ComponentModel;
using System;
using Microsoft.Extensions.DependencyInjection;
using HyPlayer.PlayCore.Abstraction.Interfaces.NotificationHub;
using HyPlayer.PlayCore.Abstraction.Models.Notifications;
using HyPlayer.PlayCore.Implementation.AudioGraphService.Abstractions;
using HyPlayer.PlayCore.Implementation.AudioGraphService.Abstractions.Notifications;
using Microsoft.UI.Dispatching;
using System.Threading;
using System.Threading.Tasks;

namespace HyPlayer.PlayCore.Demo.AudioGraph.WinUI3
{
    public partial class PositionNotificationSubscriber : ObservableObject, INotificationSubscriber<PlaybackPositionChangedNotification>
    {
        private DispatcherQueue? _dispatcherQueue;
        private readonly IServiceProvider _services;
        public Task HandleNotificationAsync(PlaybackPositionChangedNotification notification, CancellationToken ctk = default)
        {
            if (Sliding) return Task.CompletedTask;
            _dispatcherQueue ??= _services.GetService<DispatcherQueue>() ?? App.Window?.DispatcherQueue;
            _dispatcherQueue?.TryEnqueue(() =>
            {
                Position = notification.CurrentPlaybackPosition;
            });
            return Task.CompletedTask;
        }
        public PositionNotificationSubscriber(IServiceProvider services)
        {
            _services = services;
        }
        [ObservableProperty]
        private double _position;
        public bool Sliding { get; set; } = false;
    }
    public partial class MasterTicketNotificationSubscriber : ObservableObject, INotificationSubscriber<MasterTicketChangedNotification>
    {
        private DispatcherQueue? _dispatcherQueue;
        private readonly IServiceProvider _services;
        public Task HandleNotificationAsync(MasterTicketChangedNotification notification, CancellationToken ctk = default)
        {
            _dispatcherQueue ??= _services.GetService<DispatcherQueue>() ?? App.Window?.DispatcherQueue;
            _dispatcherQueue?.TryEnqueue(() =>
            {
                AudioGraphTicket = notification.MasterTicket as AudioGraphTicket;
            });
            return Task.CompletedTask;
        }
        public MasterTicketNotificationSubscriber(IServiceProvider services)
        {
            _services = services;
        }
        [ObservableProperty]
        private AudioGraphTicket _audioGraphTicket;
    }
    public partial class OnTicketReachesEndNotificationSubscriber : ObservableObject, INotificationSubscriber<AudioTicketReachesEndNotification>
    {
        private DispatcherQueue? _dispatcherQueue;
        private readonly IServiceProvider _services;
        public Task HandleNotificationAsync(AudioTicketReachesEndNotification notification, CancellationToken ctk = default)
        {
            _dispatcherQueue ??= _services.GetService<DispatcherQueue>() ?? App.Window?.DispatcherQueue;
            _dispatcherQueue?.TryEnqueue(() =>
            {
                AudioGraphTicketReachesEndName = $"{notification.AudioGraphTicket.MusicResource.ResourceName} Reaches End.";
            });
            return Task.CompletedTask;
        }
        public OnTicketReachesEndNotificationSubscriber(IServiceProvider services)
        {
            _services = services;
        }
        [ObservableProperty]
        private string _audioGraphTicketReachesEndName = string.Empty;
    }
}
