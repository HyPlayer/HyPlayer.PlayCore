using Depository.Abstraction.Interfaces.NotificationHub;
using Depository.Abstraction.Interfaces;
using HyPlayer.PlayCore.Abstraction.Models.AudioServiceComponents;
using HyPlayer.PlayCore.Implementation.AudioGraphService.Abstractions.Notifications;
using HyPlayer.PlayCore.Implementation.AudioGraphService.Abstractions;
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
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.Storage;
using Microsoft.UI.Dispatching;
using Depository.Extensions;
using Depository.Core;

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
        private AudioTicketBase _audioGraphTicket => _masterTicketChangedNotification?.AudioGraphTicket;
        private PositionNotificationSubscriber _positionNotificationSubscriber;
        private MasterTicketNotificationSubscriber _masterTicketChangedNotification;
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
            _positionNotificationSubscriber = _depository.Resolve<INotificationSubscriber<PlaybackPositionChangedNotification>>() as  PositionNotificationSubscriber;
            _masterTicketChangedNotification = _depository.Resolve<INotificationSubscriber<MasterTicketChangedNotification>>() as MasterTicketNotificationSubscriber;


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
            _audioGraphService.SeekAudioTicketAsync(_audioGraphTicket, Timeline.Value);
            _positionNotificationSubscriber.Sliding = false;
        }

        private void Timeline_ManipulationStarting(object sender, ManipulationStartingRoutedEventArgs e)
        {
            _audioGraphService.SeekAudioTicketAsync(_audioGraphTicket, Timeline.Value);
        }
    }
}
