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
using Depository.Core;
using Depository.Extensions;
using Depository.Abstraction.Interfaces;
using HyPlayer.PlayCore.Abstraction;
using HyPlayer.PlayCore.Implementation.AudioGraphService;
using HyPlayer.PlayCore.PlayListControllers;
using HyPlayer.PlayCore.Wrapper;
using Depository.Abstraction.Models.Options;
using HyPlayer.PlayCore.Abstraction.Interfaces.Wrapper;



namespace HyPlayer.PlayCore.DemoApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private Window? _window;

        public static IDepository Services { get; private set; }

        public static TService GetService<TService>()
            where TService : class
        {
            try
            {
                return Services.Resolve<TService>();
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
            Action<DepositoryOption> option = (opt)
                => {
                    opt.AutoNotifyDependencyChange = true;
                    opt.CheckerOption = new DepositoryCheckerOption()
                    {
                        AutoConstructor = true,
                        
                    };
                };
            var container = DepositoryFactory.CreateNew(option);
            // register wrapper so Chopin can be constructed
            // container.AddSingleton<IPlayCoreWrapper, PlayCoreWrapper>();
            //container.AddSingleton<PlayCoreBase, Chopin>();
            container.AddSingleton<Class1>();
            container.AddSingleton<MainWindow>();
            Services = container;
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            /*
            var playCore = (Chopin) GetService<PlayCoreBase>();
            if (playCore != null)
            {
                await playCore.RegisterAudioServiceAsync(typeof(AudioGraphService));
                await playCore.RegisterPlayControllerAsync(typeof(OrderedRollPlayController));
            }
            */
            _window = (Window) GetService<MainWindow>();
            _window.Activate();
        }
    }
}
