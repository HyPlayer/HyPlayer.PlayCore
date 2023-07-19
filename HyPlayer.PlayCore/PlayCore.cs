using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AsyncAwaitBestPractices;
using Depository.Abstraction.Interfaces;
using HyPlayer.PlayCore.Abstraction;
using HyPlayer.PlayCore.Services;

namespace HyPlayer.PlayCore;

public sealed partial class Chopin : PlayCoreBase, INotifyPropertyChanged
{
    public Chopin(
        IEnumerable<AudioServiceBase> audioServices,
        IEnumerable<ProviderBase> providers,
        IEnumerable<PlayListControllerBase> playListControllers,
        IDepository depository,
        ISmartDispatcher smartDispatcher)
    {
        _smartDispatcher = smartDispatcher;
        SafeFireAndForgetExtensions.Initialize(false);
        _depository = depository;
        AudioServices =
            new(
                new ObservableCollection<AudioServiceBase>(audioServices.ToList()));
        MusicProviders =
            new(
                new ObservableCollection<ProviderBase>(providers.ToList()));
        PlayListControllers =
            new(
                new ObservableCollection<PlayListControllerBase>(playListControllers.ToList()));
    }

    private readonly ISmartDispatcher _smartDispatcher;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        _smartDispatcher.InvokeAsync(() => { PropertyChanged?.Invoke(this, new(propertyName)); }); }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}