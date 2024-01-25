using HyPlayer.PlayCore.Implementation.AudioGraphService;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.Storage;
using HyPlayer.PlayCore.Abstraction.Models.Resources;
using HyPlayer.PlayCore.Implementation.AudioGraphService.Abstractions;
using HyPlayer.PlayCore.Abstraction.Models.AudioServiceComponents;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HyPlayer.PlayCore.Demo.AudioGraph.WinUI3
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private AudioGraphService _audioGraphService;
        private AudioTicketBase _audioGraphTicket;
        public MainWindow(AudioGraphService audioGraphService)
        {
            _audioGraphService = audioGraphService;
            this.InitializeComponent();
        }

        private async void SelectSong_Click(object sender, RoutedEventArgs e)
        {
            var file = await PickFileAsync();
            var musicResource = new AudioGraphMusicResource() { ExtensionName = file.FileType, HasContent = true, ResourceName = file.Name, Url = file.Path };
            _audioGraphTicket = await _audioGraphService.GetAudioTicketAsync(musicResource);
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            _audioGraphService?.Start();
            _audioGraphService?.PlayAudioTicketAsync(_audioGraphTicket);
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            _audioGraphService?.StopTicketAsync(_audioGraphTicket);
        }
        private async Task<StorageFile> PickFileAsync()
        {
            var filePicker = new FileOpenPicker();
            filePicker.ViewMode = PickerViewMode.Thumbnail;
            filePicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
            filePicker.FileTypeFilter.Add(".flac");
            filePicker.FileTypeFilter.Add(".mp3");
            filePicker.FileTypeFilter.Add(".wav");

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

            WinRT.Interop.InitializeWithWindow.Initialize(filePicker, hwnd);

            StorageFile file = await filePicker.PickSingleFileAsync();
            if (file != null)
            {
                // Application now has read/write access to the picked file
                return file;
            }
            else
            {
                return null;
            }
        }
    }
}
