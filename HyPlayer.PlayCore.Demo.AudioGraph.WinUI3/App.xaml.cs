using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Models;
using Depository.Core;
using Depository.Extensions;
using HyPlayer.PlayCore.Abstraction;
using HyPlayer.PlayCore.Implementation.AudioGraphService;
using HyPlayer.PlayCore.Implementation.AudioGraphService.Abstractions;
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
using Windows.Media.Audio;
using Windows.Media.Render;

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
            var container = DepositoryFactory.CreateNew();
            var settings = new AudioGraphServiceSettings() { AudioGraphSettings = new AudioGraphSettings(AudioRenderCategory.Media)};
            container.AddImplementation(typeof(AudioGraphServiceSettings), settings);
            container.AddSingleton<IAudioService,AudioGraphService>();
            container.AddSingleton<MainWindow>();
            Services = container;
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = Services.Resolve<MainWindow>();
            m_window.Activate();
        }

        private Window m_window;
        public IDepository Services { get; private set; }
    }
}
