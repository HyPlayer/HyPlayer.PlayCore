using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System;
using Microsoft.Extensions.DependencyInjection;
using HyPlayer.PlayCore.Abstraction.Interfaces.NotificationHub;
using HyPlayer.PlayCore.Abstraction;
using HyPlayer.PlayCore.Implementation.AudioGraphService;
using HyPlayer.PlayCore.PlayListControllers;
using HyPlayer.PlayCore.Wrapper;
using Depository.Abstraction.Models.Options;
using HyPlayer.PlayCore.Abstraction.Interfaces.Wrapper;
using HyPlayer.PlayCore.Abstraction.Interfaces.AudioServices;
using HyPlayer.PlayCore.Implementation.AudioGraphService.Abstractions;
using NotificationHub = HyPlayer.PlayCore.Wrapper.NotificationHub;
using INotificationHub = HyPlayer.PlayCore.Abstraction.Interfaces.NotificationHub.INotificationHub;


namespace HyPlayer.PlayCore.DemoApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private Window? _window;

        public static IServiceProvider Services { get; private set; }

        public static TService GetService<TService>()
            where TService : class
        {
            try
            {
                return Services.GetRequiredService<TService>();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unable to resolve service {typeof(TService).FullName}", ex);
            }
        }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            var services = new ServiceCollection();
            // register wrapper and notification hub as singletons but initialize them after construction
            services.AddSingleton<IPlayCoreWrapper, PlayCoreWrapper>();
            services.AddSingleton<INotificationHub, HyPlayer.PlayCore.Wrapper.NotificationHub>();

            services.AddSingleton<PlayCoreBase, Chopin>();
            services.AddSingleton<Class1>();
            services.AddSingleton<MainWindow>();

            Services = services.BuildServiceProvider();

            // Resolve the wrapper and notification hub instances and initialize them to avoid circular constructor dependency
            try
            {
                var wrapper = Services.GetService<IPlayCoreWrapper>() as PlayCoreWrapper;
                var hub = Services.GetService<INotificationHub>() as HyPlayer.PlayCore.Wrapper.NotificationHub;
                if (wrapper != null && hub != null)
                {
                    // link them after both instances are created
                    wrapper.Initialize(hub);
                    hub.Initialize(wrapper);
                }
            }
            catch
            {
                // If resolution fails here, leave as-is; the container will resolve lazily later.
            }
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            
            var playCore = (Chopin) GetService<PlayCoreBase>();
            if (playCore != null)
            {
                await playCore.RegisterAudioServiceAsync(typeof(AudioGraphService), typeof(AudioGraphServiceSettings));
                await playCore.RegisterPlayControllerAsync(typeof(OrderedRollPlayController));
            }
            
            _window = (Window) GetService<MainWindow>();
            _window.Activate();
        }
    }
}
