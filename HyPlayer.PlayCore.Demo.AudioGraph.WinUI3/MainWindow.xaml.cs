using CommunityToolkit.Mvvm.ComponentModel;
using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Interfaces.NotificationHub;
using Depository.Core;
using Depository.Extensions;
using HyPlayer.PlayCore.Abstraction.Models.AudioServiceComponents;
using HyPlayer.PlayCore.Implementation.AudioGraphService;
using HyPlayer.PlayCore.Implementation.AudioGraphService.Abstractions;
using HyPlayer.PlayCore.Implementation.AudioGraphService.Abstractions.Notifications;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HyPlayer.PlayCore.Demo.AudioGraph.WinUI3
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public IDepository Depository { get; set; }
        public MainWindow(IDepository depository)
        {
            this.InitializeComponent();
            Depository = depository;
            depository.AddSingleton<DispatcherQueue>(DispatcherQueue);
        }
        public void Navigate()
        {
            MainFrame.Navigate(typeof(BlankPage1), Depository);
        }
    }
}
