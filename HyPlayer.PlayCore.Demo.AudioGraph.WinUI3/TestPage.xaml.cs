using Depository.Abstraction.Interfaces;
using Depository.Abstraction.Interfaces.NotificationHub;
using Depository.Extensions;
using HyPlayer.PlayCore.Implementation.AudioGraphService;
using HyPlayer.PlayCore.Implementation.AudioGraphService.Abstractions;
using HyPlayer.PlayCore.Implementation.AudioGraphService.Abstractions.Notifications;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
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
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BlankPage1 : Page
    {
        private AudioGraphService _audioGraphService;
        private PositionNotificationSubscriber _positionNotificationSubscriber;
        private MasterTicketNotificationSubscriber _masterTicketChangedNotification;
        private OnTicketReachesEndNotificationSubscriber _audioTicketReachesEndNotification;
        private IDepository _depository;
        public BlankPage1()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _depository = e.Parameter as IDepository;
            _audioGraphService = _depository.Resolve<AudioGraphService>();
            _positionNotificationSubscriber = _depository.Resolve<INotificationSubscriber<PlaybackPositionChangedNotification>>() as PositionNotificationSubscriber;
            _masterTicketChangedNotification = _depository.Resolve<INotificationSubscriber<MasterTicketChangedNotification>>() as MasterTicketNotificationSubscriber;
            _audioTicketReachesEndNotification = _depository.Resolve<INotificationSubscriber<AudioTicketReachesEndNotification>>() as OnTicketReachesEndNotificationSubscriber;
            OutgoingVolume.Value = 100;
            SongVolume.Value = 100;
        }
        private async void SelectSong_Click(object sender, RoutedEventArgs e)
        {
            var file = await PickFileAsync();
            if (file == null)
            {
                return;
            }
            var musicResource = new AudioGraphMusicResource() { ExtensionName = file.FileType, HasContent = true, ResourceName = file.Name, Uri = new Uri(file.Path) };
            var result = await musicResource.GetResourceAsync();
            if (result.ResourceStatus != Abstraction.Models.ResourceStatus.Success)
            {
                throw result.ExternalException;
            }
            var result1 = await _audioGraphService.GetAudioTicketAsync(musicResource);
            await _audioGraphService.SetMasterTicketAsync(result1 as AudioGraphTicket);
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            var value = (Songs.SelectedItem as ComboBoxItem)?.Tag as AudioGraphTicket;
            if (value == null) return;
            _audioGraphService?.PlayAudioTicketAsync(value);
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            var value = (Songs.SelectedItem as ComboBoxItem)?.Tag as AudioGraphTicket;
            if (value == null) return;
            _audioGraphService?.PauseAudioTicketAsync(value);
        }
        private async Task<StorageFile> PickFileAsync()
        {
            var filePicker = new FileOpenPicker();
            filePicker.ViewMode = PickerViewMode.Thumbnail;
            filePicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
            filePicker.FileTypeFilter.Add(".flac");
            filePicker.FileTypeFilter.Add(".mp3");
            filePicker.FileTypeFilter.Add(".wav");

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.Window);

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
                var outputDevice = new AudioGraphOutputDevice() { DeviceInformation = device, Name = device.Name };
                await _audioGraphService.SetOutputDevicesAsync(outputDevice);
            }
        }

        private async void Default_Click(object sender, RoutedEventArgs e)
        {
            await _audioGraphService.SetOutputDevicesAsync(null);
        }

        private void Timeline_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            _positionNotificationSubscriber.Sliding = true;

        }

        private void Timeline_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            var value = (Songs.SelectedItem as ComboBoxItem)?.Tag as AudioGraphTicket;
            if (value == null) return;
            _audioGraphService.SeekAudioTicketAsync(value, Timeline.Value);
            _positionNotificationSubscriber.Sliding = false;
        }

        private async void AddOnline_Click(object sender, RoutedEventArgs e)
        {
            var uri = new Uri(Url.Text);
            var resource = new AudioGraphMusicResource() { ExtensionName = "Unknown", Uri = uri, HasContent = true, ResourceName = "Unknown" };
            var ticket = await _audioGraphService.GetAudioTicketAsync(resource) as AudioGraphTicket;
            await _audioGraphService.SetMasterTicketAsync(ticket);
        }

        private void Timeline_ManipulationStarting(object sender, ManipulationStartingRoutedEventArgs e)
        {
            var value = (Songs.SelectedItem as ComboBoxItem)?.Tag as AudioGraphTicket;
            if (value == null) return;
            _audioGraphService.SeekAudioTicketAsync(value, Timeline.Value);
        }

        private void OutgoingVolume_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (e.NewValue != _audioGraphService.OutgoingVolume)
            {
                _audioGraphService.ChangeOutgoingVolumeAsync(e.NewValue / 100);
            }
        }

        private void Dispose_Click(object sender, RoutedEventArgs e)
        {
            _audioGraphService.Dispose();
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            Songs.Items.Clear();
            var values = await _audioGraphService.GetCreatedAudioTicketsAsync();
            foreach (var tickets in values)
            {
                if (tickets is AudioGraphTicket ticket)
                {
                    Songs.Items.Add(new ComboBoxItem() { Content = ticket.MusicResource.ResourceName, Tag = ticket });
                }
            }
        }

        private async void Songs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Songs.SelectedItem == null || SetMasterTicket.IsChecked == false) return;
            await _audioGraphService.SetMasterTicketAsync((Songs.SelectedItem as ComboBoxItem).Tag as AudioGraphTicket);
        }

        private void StartAudioGraph_Click(object sender, RoutedEventArgs e)
        {
            _audioGraphService?.Start();
        }

        private void StopAudioGraph_Click(object sender, RoutedEventArgs e)
        {
            _audioGraphService?.Stop();
        }

        private void SongVolume_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            var value = (Songs.SelectedItem as ComboBoxItem)?.Tag as AudioGraphTicket;
            if (value == null) return;
            if (e.NewValue != value.OutgoingVolume)
            {
                _audioGraphService.ChangeVolumeAsync(value, e.NewValue / 100);
            }
        }

        private void DisposeSong_Click(object sender, RoutedEventArgs e)
        {
            var value = (Songs.SelectedItem as ComboBoxItem)?.Tag as AudioGraphTicket;
            if (value == null) return;
            _audioGraphService.DisposeAudioTicketAsync(value);
        }
    }
}
