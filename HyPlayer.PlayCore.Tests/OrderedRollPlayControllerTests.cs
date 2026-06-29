using HyPlayer.PlayCore.Abstraction.Models.Notifications;
using HyPlayer.PlayCore.Abstraction.Models.SingleItems;
using HyPlayer.PlayCore.PlayListControllers;

namespace HyPlayer.PlayCore.Tests;

public class OrderedRollPlayControllerTests
{
    [Test]
    public async Task MoveToIndex_WithValidIndex_ReturnsSongAndUpdatesCurrentIndex()
    {
        var first = new TestSong { Name = "First", ActualId = "1" };
        var second = new TestSong { Name = "Second", ActualId = "2" };
        var playlist = new TestPlayListManager([first, second]);
        var controller = CreateController(playlist);

        var selected = await controller.MoveToIndexAsync(1);

        TestAssert.Ensure(ReferenceEquals(selected, second), "MoveToIndexAsync should return the song at the requested valid index.");
        TestAssert.Ensure(await controller.GetCurrentIndexAsync() == 1, "MoveToIndexAsync should persist the selected index.");
    }

    [Test]
    public async Task MoveToIndexAndGetSongAt_WithInvalidIndexes_ReturnNullInsteadOfThrowing()
    {
        var song = new TestSong { Name = "Only", ActualId = "1" };
        var controller = CreateController(new TestPlayListManager([song]));

        var negative = await controller.MoveToIndexAsync(-1);
        var tooLarge = await controller.GetSongAtAsync(1);

        TestAssert.Ensure(negative is null, "MoveToIndexAsync should reject negative indexes.");
        TestAssert.Ensure(tooLarge is null, "GetSongAtAsync should reject indexes equal to Count.");
    }

    [Test]
    public async Task MoveNextAndMovePrevious_WrapAroundPlaylistBoundaries()
    {
        var first = new TestSong { Name = "First", ActualId = "1" };
        var second = new TestSong { Name = "Second", ActualId = "2" };
        var controller = CreateController(new TestPlayListManager([first, second]));

        var firstNext = await controller.MoveNextAsync();
        var secondNext = await controller.MoveNextAsync();
        var wrappedNext = await controller.MoveNextAsync();
        var previousFromFirst = await controller.MovePreviousAsync();

        TestAssert.Ensure(ReferenceEquals(firstNext, first), "Initial index -1 means the first next operation selects index 0.");
        TestAssert.Ensure(ReferenceEquals(secondNext, second), "The second next operation should select index 1.");
        TestAssert.Ensure(ReferenceEquals(wrappedNext, first), "Next should wrap from the last song to the first song.");
        TestAssert.Ensure(ReferenceEquals(previousFromFirst, second), "Previous should wrap from the first song to the last song.");
    }

    [Test]
    public async Task NavigateSongTo_WhenSongExists_UpdatesIndexWithoutChangingList()
    {
        var first = new TestSong { Name = "First", ActualId = "1" };
        var second = new TestSong { Name = "Second", ActualId = "2" };
        var controller = CreateController(new TestPlayListManager([first, second]));

        var selected = await controller.NavigateSongToAsync(second);
        var ordered = await controller.GetOrderedPlayListAsync();

        TestAssert.Ensure(ReferenceEquals(selected, second), "NavigateSongToAsync should return the requested song when it exists.");
        TestAssert.Ensure(await controller.GetCurrentIndexAsync() == 1, "NavigateSongToAsync should move the index to the requested song.");
        TestAssert.Ensure(ordered.SequenceEqual([first, second]), "NavigateSongToAsync should not reorder the playlist.");
    }

    [Test]
    public async Task NavigateSongTo_WhenSongMissing_DoesNotChangeIndexOrPublishInvalidSong()
    {
        var first = new TestSong { Name = "First", ActualId = "1" };
        var second = new TestSong { Name = "Second", ActualId = "2" };
        var missing = new TestSong { Name = "Missing", ActualId = "missing" };
        var notificationHub = new NoopNotificationHub();
        var controller = CreateController(new TestPlayListManager([first, second]), notificationHub);

        await controller.MoveToIndexAsync(0);
        notificationHub.PublishedNotifications.Clear();
        var selected = await controller.NavigateSongToAsync(missing);

        TestAssert.Ensure(selected is null, "NavigateSongToAsync should return null for missing songs.");
        TestAssert.Ensure(await controller.GetCurrentIndexAsync() == 0, "Missing navigation should keep the previous index.");
        TestAssert.Ensure(notificationHub.PublishedNotifications.Count == 0, "Missing navigation should not publish a fake current-song notification.");
    }

    [Test]
    public async Task Reverse_ReversesListAndTransformsIndexToSameSongPosition()
    {
        var first = new TestSong { Name = "First", ActualId = "1" };
        var second = new TestSong { Name = "Second", ActualId = "2" };
        var third = new TestSong { Name = "Third", ActualId = "3" };
        var controller = CreateController(new TestPlayListManager([first, second, third]));

        await controller.MoveToIndexAsync(1);
        await controller.Reverse();
        var ordered = await controller.GetOrderedPlayListAsync();

        TestAssert.Ensure(ordered.SequenceEqual([third, second, first]), "Reverse should reverse the playlist order.");
        TestAssert.Ensure(await controller.GetCurrentIndexAsync() == 1, "The middle song should remain selected after reversing a three-item playlist.");
    }

    [Test]
    public async Task RandomizeAndRestore_UsesDeterministicShuffleAndOriginalOrder()
    {
        var first = new TestSong { Name = "First", ActualId = "1" };
        var second = new TestSong { Name = "Second", ActualId = "2" };
        var third = new TestSong { Name = "Third", ActualId = "3" };
        var controller = CreateController(new TestPlayListManager([first, second, third]));

        await controller.RandomizeAsync(42);
        var randomized = await controller.GetOrderedPlayListAsync();
        await controller.RandomizeAsync(-1);
        var restored = await controller.GetOrderedPlayListAsync();

        TestAssert.Ensure(randomized.Count == 3 && randomized.ToHashSet().SetEquals([first, second, third]), "RandomizeAsync should keep the same songs.");
        TestAssert.Ensure(restored.SequenceEqual([first, second, third]), "RandomizeAsync(-1) should restore the original order.");
    }

    [Test]
    public async Task Randomize_WhenPlaylistChanges_KeepsRandomModeAndCurrentSong()
    {
        var first = new TestSong { Name = "First", ActualId = "1" };
        var second = new TestSong { Name = "Second", ActualId = "2" };
        var third = new TestSong { Name = "Third", ActualId = "3" };
        var fourth = new TestSong { Name = "Fourth", ActualId = "4" };
        var songs = new List<SingleSongBase> { first, second, third };
        var notificationHub = new NoopNotificationHub();
        var controller = CreateController(new TestPlayListManager(songs), notificationHub);

        await controller.MoveToIndexAsync(1);
        await controller.RandomizeAsync(42);
        notificationHub.PublishedNotifications.Clear();
        songs.Add(fourth);
        await controller.HandleNotificationAsync(new());
        var ordered = await controller.GetOrderedPlayListAsync();

        TestAssert.Ensure(ordered.ToHashSet().SetEquals([first, second, third, fourth]), "Playlist refresh should include the updated original playlist while random mode is enabled.");
        TestAssert.Ensure(ReferenceEquals(ordered[await controller.GetCurrentIndexAsync()], second), "Playlist refresh should keep the current song selected when it still exists.");
        TestAssert.Ensure(notificationHub.PublishedNotifications.OfType<OrderedPlaylistChangedNotification>().Single().IsRandom, "Playlist refresh should preserve random mode instead of publishing a sequential playlist.");
    }

    [Test]
    public async Task EmptyPlaylist_NavigationMethodsReturnNullAndNavigationDoesNotThrow()
    {
        var controller = CreateController(new TestPlayListManager([]));

        var next = await controller.MoveNextAsync();
        var previous = await controller.MovePreviousAsync();
        await controller.NavigateSongToAsync(new TestSong { Name = "Missing", ActualId = "missing" });

        TestAssert.Ensure(next is null, "MoveNextAsync should return null for an empty playlist.");
        TestAssert.Ensure(previous is null, "MovePreviousAsync should return null for an empty playlist.");
    }

    [Test]
    public async Task InnerPlaylistChangedNotification_RefreshesOrderedListFromPlaylistManager()
    {
        var first = new TestSong { Name = "First", ActualId = "1" };
        var playlist = new TestPlayListManager([first]);
        var controller = CreateController(playlist);

        await controller.HandleNotificationAsync(new());
        var ordered = await controller.GetOrderedPlayListAsync();

        TestAssert.Ensure(ordered.SequenceEqual([first]), "InnerPlayListChangedNotification should reload the ordered playlist from PlayListManager.");
    }

    private static OrderedRollPlayController CreateController(TestPlayListManager playlist, NoopNotificationHub? notificationHub = null)
    {
        var controller = new OrderedRollPlayController(new TestDepository(playlist), playlist, notificationHub ?? new NoopNotificationHub());
        controller.OnDependencyChanged(playlist);
        return controller;
    }
}
