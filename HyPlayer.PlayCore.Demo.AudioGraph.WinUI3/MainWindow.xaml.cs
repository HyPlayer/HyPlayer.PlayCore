using HyPlayer.PlayCore.Abstraction.Models.AudioServiceComponents;
using HyPlayer.PlayCore.Implementation.AudioGraphService;
using HyPlayer.PlayCore.Implementation.AudioGraphService.Abstractions;
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
            if (file == null)
            {
                return;
            }
            var musicResource = new AudioGraphMusicResource() { ExtensionName = file.FileType, HasContent = true, ResourceName = file.Name, Url = file.Path };
            _audioGraphTicket = await _audioGraphService.GetAudioTicketAsync(musicResource);
            await _audioGraphService.SetMasterTicketAsync(_audioGraphTicket as AudioGraphTicket);
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            _audioGraphService?.Start();
            _audioGraphService?.PlayAudioTicketAsync(_audioGraphTicket);
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            _audioGraphService?.Stop();
            _audioGraphService?.PauseAudioTicketAsync(_audioGraphTicket);
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

        private async void ChangeDevice_Click(object sender, RoutedEventArgs e)
        {
            var devicePicker = new DevicePicker();
            devicePicker.Filter.SupportedDeviceClasses.Add(DeviceClass.AudioRender);
            var ge = ChangeDevice.TransformToVisual(null);
            var point = ge.TransformPoint(new Point());
            var rect = new Rect(point,
                new Point(point.X + ChangeDevice.ActualWidth,
                    point.Y + ChangeDevice.ActualHeight));
            var device = await devicePicker.PickSingleDeviceAsync(rect);
            if (device != null)
            {
                var outputDevice = new AudioGraphOutputDevice() {DeviceInformation = device, Name = device.Name};
                await _audioGraphService.SetOutputDevicesAsync(outputDevice);
            }
        }

        private async void Default_Click(object sender, RoutedEventArgs e)
        {
            await _audioGraphService.SetOutputDevicesAsync(null);
        }
    }
}
