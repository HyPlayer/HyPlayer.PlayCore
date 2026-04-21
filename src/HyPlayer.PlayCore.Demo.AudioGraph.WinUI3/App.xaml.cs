using System;
using Microsoft.Extensions.DependencyInjection;
using HyPlayer.PlayCore.Abstraction.Interfaces.NotificationHub;
using HyPlayer.PlayCore.Wrapper;
using Microsoft.UI.Dispatching;
using HyPlayer.PlayCore.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction.Models.Notifications;
using HyPlayer.PlayCore.Implementation.AudioGraphService;
using HyPlayer.PlayCore.Implementation.AudioGraphService.Abstractions;
using HyPlayer.PlayCore.Implementation.AudioGraphService.Abstractions.Notifications;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HyPlayer.PlayCore.Demo.AudioGraph.WinUI3
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            var services = new ServiceCollection();
            var settings = new AudioGraphServiceSettings();
            services.AddSingleton<AudioServiceSettingsBase>(settings);
            services.AddSingleton<AudioGraphService>();
            services.AddSingleton<INotificationHub, NotificationHub>();
            services.AddSingleton<INotificationSubscriber<PlaybackPositionChangedNotification>, PositionNotificationSubscriber>();
            services.AddSingleton<INotificationSubscriber<MasterTicketChangedNotification>, MasterTicketNotificationSubscriber>();
            services.AddSingleton<INotificationSubscriber<AudioTicketReachesEndNotification>, OnTicketReachesEndNotificationSubscriber>();
            services.AddSingleton<MainWindow>();
            // register DispatcherQueue factory so it can be resolved by consumers
            services.AddSingleton<DispatcherQueue>(_ => DispatcherQueue.GetForCurrentThread());
            Services = services.BuildServiceProvider();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            var window = Services.GetRequiredService<MainWindow>();
            Window = window;
            Window.Activate();
            window.Navigate();
        }
        public static Window Window;
        public static IServiceProvider Services { get; private set; }
    }
}
