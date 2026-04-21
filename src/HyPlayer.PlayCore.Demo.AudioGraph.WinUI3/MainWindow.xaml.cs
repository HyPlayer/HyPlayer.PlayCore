using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HyPlayer.PlayCore.Demo.AudioGraph.WinUI3
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public IServiceProvider Services { get; }
        public MainWindow(IServiceProvider services)
        {
            this.InitializeComponent();
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }
        public void Navigate()
        {
            MainFrame.Navigate(typeof(BlankPage1), Services);
        }
    }
}
