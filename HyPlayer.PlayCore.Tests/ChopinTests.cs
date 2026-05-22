using HyPlayer.PlayCore.Abstraction;
using HyPlayer.PlayCore.Abstraction.Models.AudioServiceComponents;
using HyPlayer.PlayCore.Abstraction.Models.Notifications;

namespace HyPlayer.PlayCore.Tests;

public class ChopinTests
{
    [Test]
    public async Task Constructor_WithInitialServices_SelectsFirstAudioServiceAndPlaylistControllerAndResolvesPlaylist()
    {
        var audioService = new TestAudioService();
        var controller = new TestPlayController(null);
        var playlist = new TestPlayListManager([]);

        var core = new Chopin([audioService], [], [controller], new TestDepository(playlist));

        TestAssert.Ensure(ReferenceEquals(core.CurrentAudioService, audioService), "The first constructor audio service should become CurrentAudioService.");
        TestAssert.Ensure(ReferenceEquals(core.CurrentPlayListController, controller), "The first constructor playlist controller should become CurrentPlayListController.");
        TestAssert.Ensure(ReferenceEquals(core.CurrentPlayList, playlist), "Constructor should resolve CurrentPlayList from Depository when one is registered.");
    }

    [Test]
    public async Task MoveNextThenPlay_WithMatchingProvider_CreatesCurrentTicketAndInvokesAudioService()
    {
        var song = new TestSong { Name = "Song", ActualId = "1", Duration = 1000, Available = true };
        var resource = new TestMusicResource();
        var provider = new TestProvider(resource);
        var audioService = new TestAudioService();
        var controller = new TestPlayController(song);
        var core = new Chopin([audioService], [provider], [controller], new TestDepository());

        await core.MoveNextAsync();
        await core.PlayAsync();

        TestAssert.Ensure(ReferenceEquals(core.CurrentSong, song), "MoveNextAsync should copy the selected song into CurrentSong.");
        TestAssert.Ensure(core.CurrentPlayingTicket is not null, "PlayAsync should create CurrentPlayingTicket for CurrentSong.");
        TestAssert.Ensure(audioService.CreatedTicketCount == 1, "Exactly one ticket should be created for the first play.");
        TestAssert.Ensure(ReferenceEquals(audioService.PlayedTicket, core.CurrentPlayingTicket), "The newly created ticket should be passed to the audio service.");
    }

    [Test]
    public async Task PlayAsync_CalledTwiceForSameCurrentSong_ReusesCurrentTicket()
    {
        var song = new TestSong { Name = "Song", ActualId = "1" };
        var audioService = new TestAudioService();
        var core = new Chopin([audioService], [new TestProvider(new TestMusicResource())], [new TestPlayController(song)], new TestDepository());

        await core.MoveNextAsync();
        await core.PlayAsync();
        var firstTicket = core.CurrentPlayingTicket;
        await core.PlayAsync();

        TestAssert.Ensure(audioService.CreatedTicketCount == 1, "Repeated PlayAsync for the same CurrentSong should not create duplicate tickets.");
        TestAssert.Ensure(ReferenceEquals(core.CurrentPlayingTicket, firstTicket), "CurrentPlayingTicket should be reused for the same CurrentSong.");
    }

    [Test]
    public async Task PlayAsync_AfterCurrentSongChanges_DisposesOldTicketAndCreatesNewTicket()
    {
        var first = new TestSong { Name = "First", ActualId = "1" };
        var second = new TestSong { Name = "Second", ActualId = "2" };
        var controller = new SequencePlayController([first, second]);
        var audioService = new TestAudioService();
        var core = new Chopin([audioService], [new TestProvider(new TestMusicResource())], [controller], new TestDepository());

        await core.MoveNextAsync();
        await core.PlayAsync();
        var firstTicket = core.CurrentPlayingTicket;
        await core.MoveNextAsync();
        await core.PlayAsync();

        TestAssert.Ensure(audioService.CreatedTicketCount == 2, "A new CurrentSong should receive a new ticket.");
        TestAssert.Ensure(firstTicket is not null && audioService.DisposedTickets.Contains(firstTicket), "The old ticket should be disposed before replacing CurrentPlayingTicket.");
        TestAssert.Ensure(!ReferenceEquals(firstTicket, core.CurrentPlayingTicket), "CurrentPlayingTicket should point to the new song ticket.");
    }

    [Test]
    public async Task PlayAsync_WhenProviderDoesNotMatchSong_DoesNotCreateTicket()
    {
        var song = new OtherProviderSong { Name = "Song", ActualId = "1" };
        var audioService = new TestAudioService();
        var notificationHub = new NoopNotificationHub();
        var core = new Chopin([audioService], [new TestProvider(new TestMusicResource())], [new TestPlayController(song)], new TestDepository(notificationHub));

        await core.MoveNextAsync();
        await core.PlayAsync();

        TestAssert.Ensure(core.CurrentPlayingTicket is null, "No ticket should be created without a matching provider.");
        TestAssert.Ensure(audioService.CreatedTicketCount == 0, "AudioService should not be invoked when provider resolution fails.");
        TestAssert.Ensure(notificationHub.PublishedNotifications.OfType<PlaybackRequestFailedNotification>().Single().Song == song,
            "Provider resolution failure should publish PlaybackRequestFailedNotification for the requested song.");
    }

    [Test]
    public async Task PlayAsync_WhenProviderReturnsNullResource_DoesNotCreateTicket()
    {
        var song = new TestSong { Name = "Song", ActualId = "1" };
        var audioService = new TestAudioService();
        var notificationHub = new NoopNotificationHub();
        var core = new Chopin([audioService], [new TestProvider(null)], [new TestPlayController(song)], new TestDepository(notificationHub));

        await core.MoveNextAsync();
        await core.PlayAsync();

        TestAssert.Ensure(core.CurrentPlayingTicket is null, "No ticket should be created when resource resolution returns null.");
        TestAssert.Ensure(audioService.CreatedTicketCount == 0, "AudioService should not receive a null music resource.");
        TestAssert.Ensure(notificationHub.PublishedNotifications.OfType<PlaybackRequestFailedNotification>().Any(),
            "Null resource resolution should publish a playback failure notification.");
    }

    [Test]
    public async Task SeekPauseStop_WithCurrentTicket_ForwardsToCurrentAudioService()
    {
        var song = new TestSong { Name = "Song", ActualId = "1" };
        var audioService = new TestAudioService();
        var core = new Chopin([audioService], [new TestProvider(new TestMusicResource())], [new TestPlayController(song)], new TestDepository());

        await core.MoveNextAsync();
        await core.PlayAsync();
        var activeTicket = core.CurrentPlayingTicket;
        await core.SeekAsync(1234);
        await core.PauseAsync();
        await core.StopAsync();

        TestAssert.Ensure(audioService.LastSeekPosition == 1234, "SeekAsync should forward the requested position.");
        TestAssert.Ensure(ReferenceEquals(audioService.PausedTicket, activeTicket), "PauseAsync should forward CurrentPlayingTicket.");
        TestAssert.Ensure(ReferenceEquals(audioService.StoppedTicket, activeTicket), "StopAsync should forward the active ticket before clearing it.");
        TestAssert.Ensure(core.CurrentPlayingTicket is null, "StopAsync should clear CurrentPlayingTicket after the audio service stops/disposes it.");
    }

    [Test]
    public async Task StopThenPlaySameSong_RecreatesTicketInsteadOfReusingStoppedTicket()
    {
        var song = new TestSong { Name = "Song", ActualId = "1" };
        var audioService = new TestAudioService();
        var core = new Chopin([audioService], [new TestProvider(new TestMusicResource())], [new TestPlayController(song)], new TestDepository());

        await core.MoveNextAsync();
        await core.PlayAsync();
        var firstTicket = core.CurrentPlayingTicket;
        await core.StopAsync();
        await core.PlayAsync();

        TestAssert.Ensure(audioService.CreatedTicketCount == 2, "PlayAsync after StopAsync should create a fresh ticket for the same song.");
        TestAssert.Ensure(!ReferenceEquals(firstTicket, core.CurrentPlayingTicket), "The stopped ticket should not be reused.");
        TestAssert.Ensure(ReferenceEquals(audioService.PlayedTicket, core.CurrentPlayingTicket), "The fresh ticket should be the one passed to PlayAudioTicketAsync.");
    }

    [Test]
    public async Task SeekAsync_WithoutCurrentSongOrTicket_DoesNothing()
    {
        var audioService = new TestAudioService();
        var core = new Chopin([audioService], [new TestProvider(new TestMusicResource())], [], new TestDepository());

        await core.SeekAsync(500);

        TestAssert.Ensure(audioService.LastSeekPosition is null, "SeekAsync should not call AudioService without a ticket.");
        TestAssert.Ensure(core.CurrentPlayingTicket is null, "SeekAsync should not create a ticket without CurrentSong.");
    }

    [Test]
    public async Task PlayAsync_WhenCurrentAudioServiceChanges_CreatesAndPlaysTicketForNewService()
    {
        var song = new TestSong { Name = "Song", ActualId = "1" };
        var firstAudioService = new TestAudioService("audio.first");
        var secondAudioService = new TestAudioService("audio.second");
        var core = new Chopin([firstAudioService], [new TestProvider(new TestMusicResource())], [new TestPlayController(song)], new TestDepository(secondAudioService));

        await core.MoveNextAsync();
        await core.PlayAsync();
        core.OnDependencyChanged((AudioServiceBase?)null);
        await core.PlayAsync();

        TestAssert.Ensure(firstAudioService.CreatedTicketCount == 1, "The original service should create the original ticket.");
        TestAssert.Ensure(secondAudioService.CreatedTicketCount == 1, "Switching CurrentAudioService should create a replacement ticket for the new service.");
        TestAssert.Ensure(ReferenceEquals(secondAudioService.PlayedTicket, core.CurrentPlayingTicket), "The replacement ticket should be played by the new current service.");
        TestAssert.Ensure(core.CurrentPlayingTicket?.AudioServiceId == secondAudioService.Id, "CurrentPlayingTicket should belong to the new current service.");
    }

    [Test]
    public async Task PlaylistModification_WithCurrentPlaylist_DelegatesEveryMutation()
    {
        var first = new TestSong { Name = "First", ActualId = "1" };
        var second = new TestSong { Name = "Second", ActualId = "2" };
        var playlist = new TestPlayListManager([]);
        var core = new Chopin([], [], [], new TestDepository(playlist));

        await core.InsertSongAsync(first);
        await core.InsertSongRangeAsync([second]);
        await core.RemoveSongAsync(first);
        await core.RemoveSongRangeAsync([second]);
        await core.RemoveAllSongAsync();

        TestAssert.Ensure(playlist.AddedSongs.SequenceEqual([first]), "InsertSongAsync should delegate to CurrentPlayList.AddSongAsync.");
        TestAssert.Ensure(playlist.AddedRanges.Single().SequenceEqual([second]), "InsertSongRangeAsync should delegate to CurrentPlayList.AddSongRangeAsync.");
        TestAssert.Ensure(playlist.RemovedSongs.SequenceEqual([first]), "RemoveSongAsync should delegate to CurrentPlayList.RemoveSongAsync.");
        TestAssert.Ensure(playlist.RemovedRanges.Single().SequenceEqual([second]), "RemoveSongRangeAsync should delegate to CurrentPlayList.RemoveSongRangeAsync.");
        TestAssert.Ensure(playlist.ClearSongsCalled == 1, "RemoveAllSongAsync should delegate to CurrentPlayList.ClearSongsAsync.");
    }

    [Test]
    public async Task ChangeSongContainer_UpdatesCurrentSongContainer()
    {
        var container = new ProgressiveSongContainer([]) { Name = "Container", ActualId = "1" };
        var core = new Chopin([], [], [], new TestDepository());

        await core.ChangeSongContainerAsync(container);

        TestAssert.Ensure(ReferenceEquals(core.CurrentSongContainer, container), "ChangeSongContainerAsync should store the selected song container.");
    }

    [Test]
    public async Task CurrentSongChangedNotification_UpdatesCurrentSong()
    {
        var song = new TestSong { Name = "Song", ActualId = "1" };
        var core = new Chopin([], [], [], new TestDepository());

        await core.HandleNotificationAsync(new CurrentSongChangedNotification { CurrentPlayingSong = song });

        TestAssert.Ensure(ReferenceEquals(core.CurrentSong, song), "CurrentSongChangedNotification should update CurrentSong.");
    }

    [Test]
    public async Task DependencyChanged_ForCurrentServices_RefreshesCurrentReferences()
    {
        var audioService = new TestAudioService();
        var controller = new TestPlayController(null);
        var playlist = new TestPlayListManager([]);
        var core = new Chopin([], [], [], new TestDepository(audioService, controller, playlist));

        core.OnDependencyChanged((AudioServiceBase?)null);
        core.OnDependencyChanged((PlayControllerBase?)null);
        core.OnDependencyChanged((PlayListManagerBase?)null);

        TestAssert.Ensure(ReferenceEquals(core.CurrentAudioService, audioService), "AudioService dependency changes should refresh CurrentAudioService.");
        TestAssert.Ensure(ReferenceEquals(core.CurrentPlayListController, controller), "PlayController dependency changes should refresh CurrentPlayListController.");
        TestAssert.Ensure(ReferenceEquals(core.CurrentPlayList, playlist), "PlayListManager dependency changes should refresh CurrentPlayList.");
    }

    [Test]
    public async Task DependencyChanged_WhenResolveFails_ClearsCurrentReferences()
    {
        var core = new Chopin([new TestAudioService()], [], [new TestPlayController(null)], new TestDepository());

        core.OnDependencyChanged((AudioServiceBase?)null);
        core.OnDependencyChanged((PlayControllerBase?)null);
        core.OnDependencyChanged((PlayListManagerBase?)null);

        TestAssert.Ensure(core.CurrentAudioService is null, "Missing AudioService dependency should clear CurrentAudioService.");
        TestAssert.Ensure(core.CurrentPlayListController is null, "Missing PlayController dependency should clear CurrentPlayListController.");
        TestAssert.Ensure(core.CurrentPlayList is null, "Missing PlayListManager dependency should clear CurrentPlayList.");
    }

    [Test]
    public async Task FocusMethods_UpdateCurrentAudioServiceAndPlaylistController()
    {
        var audioService = new TestAudioService();
        var controller = new TestPlayController(null);
        var core = new Chopin([], [], [], new TestDepository(audioService, controller));

        await core.RegisterAudioServiceAsync(typeof(TestAudioService));
        await core.RegisterPlayListControllerAsync(typeof(TestPlayController));
        await core.FocusAudioServiceAsync(typeof(TestAudioService));
        await core.FocusPlayListControllerAsync(typeof(TestPlayController));

        TestAssert.Ensure(ReferenceEquals(core.CurrentAudioService, audioService), "FocusAudioServiceAsync should update CurrentAudioService.");
        TestAssert.Ensure(ReferenceEquals(core.CurrentPlayListController, controller), "FocusPlayListControllerAsync should update CurrentPlayListController.");
    }

    [Test]
    public async Task RegisterAndUnregisterMethods_UseCorrectDepositoryRelations()
    {
        var depository = new TestDepository();
        var core = new Chopin([], [], [], depository);

        await core.RegisterAudioServiceAsync(typeof(TestAudioService));
        await core.RegisterMusicProviderAsync(typeof(TestProvider));
        await core.RegisterPlayListControllerAsync(typeof(TestPlayController));
        await core.UnregisterAudioServiceAsync(typeof(TestAudioService));
        await core.UnregisterMusicProviderAsync(typeof(TestProvider));
        await core.UnregisterPlayListControllerAsync(typeof(TestPlayController));

        TestAssert.Ensure(depository.AddedRelations.Any(r => r.DependencyType == typeof(AudioServiceBase) && r.ImplementType == typeof(TestAudioService)), "Audio service registration should add an AudioServiceBase relation.");
        TestAssert.Ensure(depository.AddedRelations.Any(r => r.DependencyType == typeof(ProviderBase) && r.ImplementType == typeof(TestProvider)), "Provider registration should add a ProviderBase relation.");
        TestAssert.Ensure(depository.AddedRelations.Any(r => r.DependencyType == typeof(PlayControllerBase) && r.ImplementType == typeof(TestPlayController)), "Playlist controller registration should add a PlayControllerBase relation.");
        TestAssert.Ensure(depository.DeletedRelations.Any(r => r.DependencyType == typeof(AudioServiceBase) && r.ImplementType == typeof(TestAudioService)), "Audio service unregister should delete the AudioServiceBase relation.");
        TestAssert.Ensure(depository.DeletedRelations.Any(r => r.DependencyType == typeof(ProviderBase) && r.ImplementType == typeof(TestProvider)), "Provider unregister should delete the ProviderBase relation.");
        TestAssert.Ensure(depository.DeletedRelations.Any(r => r.DependencyType == typeof(PlayControllerBase) && r.ImplementType == typeof(TestPlayController)), "Playlist controller unregister should delete the PlayControllerBase relation.");
    }

    [Test]
    public async Task CollectionDependencyChanged_RefreshesPublicServiceLists()
    {
        var audioService = new TestAudioService();
        var provider = new TestProvider(new TestMusicResource());
        var controller = new TestPlayController(null);
        var core = new Chopin([], [], [], new TestDepository(audioService, provider, controller));

        core.OnDependencyChanged((IEnumerable<AudioServiceBase>?)null);
        core.OnDependencyChanged((IEnumerable<ProviderBase>?)null);
        core.OnDependencyChanged((IEnumerable<PlayControllerBase>?)null);

        TestAssert.Ensure(core.AudioServices?.Single() == audioService, "AudioServices should refresh from Depository.");
        TestAssert.Ensure(core.MusicProviders?.Single() == provider, "MusicProviders should refresh from Depository.");
        TestAssert.Ensure(core.PlayListControllers?.Single() == controller, "PlayListControllers should refresh from Depository.");
    }

    [Test]
    public async Task MovePointerTo_WithNavigableController_DelegatesTargetSong()
    {
        var song = new TestSong { Name = "Song", ActualId = "1" };
        var controller = new NavigablePlayController();
        var core = new Chopin([], [], [controller], new TestDepository());

        await core.MovePointerToAsync(song);

        TestAssert.Ensure(ReferenceEquals(controller.NavigatedSong, song), "MovePointerToAsync should call NavigateSongToAsync on navigable controllers.");
    }
}
