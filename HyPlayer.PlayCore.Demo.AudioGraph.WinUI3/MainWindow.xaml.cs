using Depository.Abstraction.Interfaces;
using Depository.Extensions;
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
