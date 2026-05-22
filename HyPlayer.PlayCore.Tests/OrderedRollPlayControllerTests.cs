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
        var previousFromSecond = await controller.MovePreviousAsync();

        TestAssert.Ensure(ReferenceEquals(firstNext, second), "Initial index 0 means the first next operation selects index 1.");
        TestAssert.Ensure(ReferenceEquals(secondNext, first), "Next should wrap from the last song to the first song.");
        TestAssert.Ensure(ReferenceEquals(wrappedNext, second), "Next should continue correctly after wrapping.");
        TestAssert.Ensure(ReferenceEquals(previousFromSecond, first), "Previous should move from index 1 back to index 0.");
    }

    [Test]
    public async Task NavigateSongTo_WhenSongExists_UpdatesIndexWithoutChangingList()
    {
        var first = new TestSong { Name = "First", ActualId = "1" };
        var second = new TestSong { Name = "Second", ActualId = "2" };
        var controller = CreateController(new TestPlayListManager([first, second]));

        await controller.NavigateSongToAsync(second);
        var ordered = await controller.GetOrderedPlayListAsync();

        TestAssert.Ensure(await controller.GetCurrentIndexAsync() == 1, "NavigateSongToAsync should move the index to the requested song.");
        TestAssert.Ensure(ordered.SequenceEqual([first, second]), "NavigateSongToAsync should not reorder the playlist.");
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

    private static OrderedRollPlayController CreateController(TestPlayListManager playlist)
    {
        var controller = new OrderedRollPlayController(new TestDepository(playlist), playlist, new NoopNotificationHub());
        controller.OnDependencyChanged(playlist);
        return controller;
    }
}
