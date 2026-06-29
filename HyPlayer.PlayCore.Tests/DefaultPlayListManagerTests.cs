using HyPlayer.PlayCore.PlayListControllers;

namespace HyPlayer.PlayCore.Tests;

public class DefaultPlayListManagerTests
{
    [Test]
    public async Task AddProgressiveContainer_LoadsEveryPageAndAdvancesStartOffset()
    {
        var container = new ProgressiveSongContainer([
            new TestSong { Name = "First", ActualId = "1" },
            new TestSong { Name = "Second", ActualId = "2" }
        ]) { Name = "Progressive", ActualId = "progressive" };
        var manager = CreateManager();

        await manager.AddSongContainerAsync(container);
        var playlist = await manager.GetPlayListAsync();

        TestAssert.Ensure(container.RequestedStarts.SequenceEqual([0, 1]), "Progressive loading should request each page with an advanced start offset.");
        TestAssert.Ensure(playlist.Select(s => s.ActualId).SequenceEqual(["1", "2"]), "Progressive loading should add every returned song exactly once.");
    }

    [Test]
    public async Task AddLinerAndUndeterminedContainers_LoadsSongsIntoPlaylist()
    {
        var linerSong = new TestSong { Name = "Liner", ActualId = "1" };
        var undeterminedSong = new TestSong { Name = "Undetermined", ActualId = "2" };
        var manager = CreateManager();

        await manager.AddSongContainerAsync(new TestLinerContainer([linerSong]) { Name = "Liner", ActualId = "liner" });
        await manager.AddSongContainerAsync(new TestUndeterminedContainer([undeterminedSong]) { Name = "Undetermined", ActualId = "undetermined" });
        var playlist = await manager.GetPlayListAsync();

        TestAssert.Ensure(playlist.SequenceEqual([linerSong, undeterminedSong]), "Liner and undetermined containers should both contribute songs to the playlist.");
    }

    [Test]
    public async Task AddSongAndRange_WithAndWithoutIndexes_UpdatesPlaylistOrder()
    {
        var first = new TestSong { Name = "First", ActualId = "1" };
        var second = new TestSong { Name = "Second", ActualId = "2" };
        var third = new TestSong { Name = "Third", ActualId = "3" };
        var manager = CreateManager();

        await manager.AddSongAsync(first);
        await manager.AddSongRangeAsync([second]);
        await manager.AddSongAsync(third, 1);
        var playlist = await manager.GetPlayListAsync();

        TestAssert.Ensure(playlist.SequenceEqual([first, third, second]), "AddSongAsync(index) should insert at the requested index while AddSongRangeAsync appends by default.");
    }

    [Test]
    public async Task AddSongAndRange_WithOutOfRangeIndexes_DoNothingInsteadOfThrowing()
    {
        var first = new TestSong { Name = "First", ActualId = "1" };
        var second = new TestSong { Name = "Second", ActualId = "2" };
        var third = new TestSong { Name = "Third", ActualId = "3" };
        var manager = CreateManager();

        await manager.AddSongAsync(first);
        await manager.AddSongAsync(second, 2);
        await manager.AddSongRangeAsync([third], -2);
        var playlist = await manager.GetPlayListAsync();

        TestAssert.Ensure(playlist.SequenceEqual([first]), "Out-of-range insert indexes should be ignored instead of throwing or mutating the playlist.");
    }

    [Test]
    public async Task RemoveSongAndRange_RemoveOnlyRequestedSongs()
    {
        var first = new TestSong { Name = "First", ActualId = "1" };
        var second = new TestSong { Name = "Second", ActualId = "2" };
        var third = new TestSong { Name = "Third", ActualId = "3" };
        var manager = CreateManager();

        await manager.SetSongListAsync([first, second, third]);
        await manager.RemoveSongAsync(second);
        await manager.RemoveSongRangeAsync([third]);
        var playlist = await manager.GetPlayListAsync();

        TestAssert.Ensure(playlist.SequenceEqual([first]), "RemoveSongAsync and RemoveSongRangeAsync should remove only requested songs.");
    }

    [Test]
    public async Task SetSongListAndClearSongs_ReplaceThenEmptyPlaylist()
    {
        var first = new TestSong { Name = "First", ActualId = "1" };
        var second = new TestSong { Name = "Second", ActualId = "2" };
        var manager = CreateManager();

        await manager.AddSongAsync(first);
        await manager.SetSongListAsync([second]);
        var afterSet = (await manager.GetPlayListAsync()).ToList();
        await manager.ClearSongsAsync();
        var afterClear = await manager.GetPlayListAsync();

        TestAssert.Ensure(afterSet.SequenceEqual([second]), "SetSongListAsync should replace existing playlist content.");
        TestAssert.Ensure(afterClear.Count == 0, "ClearSongsAsync should empty the playlist.");
    }

    [Test]
    public async Task ReturnedLists_AreCopiesAndCannotMutateManagerState()
    {
        var song = new TestSong { Name = "Song", ActualId = "1" };
        var container = new TestLinerContainer([song]) { Name = "Container", ActualId = "container" };
        var manager = CreateManager();

        await manager.AddSongContainerAsync(container);
        var playlistSnapshot = await manager.GetPlayListAsync();
        var containerSnapshot = await manager.GetAllSongContainersAsync();
        playlistSnapshot.Clear();
        containerSnapshot.Clear();

        TestAssert.Ensure((await manager.GetPlayListAsync()).Single() == song, "Mutating a returned playlist snapshot should not mutate manager state.");
        TestAssert.Ensure((await manager.GetAllSongContainersAsync()).Single() == container, "Mutating a returned container snapshot should not mutate manager state.");
    }

    [Test]
    public async Task RemoveSongContainer_RemovesContainerSongsAndKeepsOtherContainerSongs()
    {
        var first = new TestSong { Name = "First", ActualId = "1" };
        var second = new TestSong { Name = "Second", ActualId = "2" };
        var firstContainer = new TestLinerContainer([first]) { Name = "FirstContainer", ActualId = "c1" };
        var secondContainer = new TestLinerContainer([second]) { Name = "SecondContainer", ActualId = "c2" };
        var manager = CreateManager();

        await manager.AddSongContainerAsync(firstContainer);
        await manager.AddSongContainerAsync(secondContainer);
        await manager.RemoveSongContainerAsync(firstContainer);
        var playlist = await manager.GetPlayListAsync();
        var containers = await manager.GetAllSongContainersAsync();

        TestAssert.Ensure(playlist.SequenceEqual([second]), "RemoveSongContainerAsync should remove only songs loaded from that container.");
        TestAssert.Ensure(containers.SequenceEqual([secondContainer]), "RemoveSongContainerAsync should remove only the requested container from the active set.");
    }

    [Test]
    public async Task ClearSongContainers_ClearsContainersAndTheirSongs()
    {
        var first = new TestSong { Name = "First", ActualId = "1" };
        var second = new TestSong { Name = "Second", ActualId = "2" };
        var manager = CreateManager();

        await manager.AddSongContainerAsync(new TestLinerContainer([first]) { Name = "FirstContainer", ActualId = "c1" });
        await manager.AddSongContainerAsync(new TestLinerContainer([second]) { Name = "SecondContainer", ActualId = "c2" });
        await manager.ClearSongContainersAsync();

        TestAssert.Ensure((await manager.GetPlayListAsync()).Count == 0, "ClearSongContainersAsync should clear playlist songs.");
        TestAssert.Ensure((await manager.GetAllSongContainersAsync()).Count == 0, "ClearSongContainersAsync should clear active containers.");
    }

    [Test]
    public async Task AddUndeterminedContainer_ReplacesExistingContainerSelection()
    {
        var linerSong = new TestSong { Name = "Liner", ActualId = "1" };
        var undeterminedSong = new TestSong { Name = "Undetermined", ActualId = "2" };
        var manager = CreateManager();

        await manager.AddSongContainerAsync(new TestLinerContainer([linerSong]) { Name = "Liner", ActualId = "liner" });
        var undetermined = new TestUndeterminedContainer([undeterminedSong]) { Name = "Undetermined", ActualId = "undetermined" };
        await manager.AddSongContainerAsync(undetermined);
        var containers = await manager.GetAllSongContainersAsync();

        TestAssert.Ensure(containers.SequenceEqual([undetermined]), "Adding an undetermined container should replace the active container selection.");
    }

    private static DefaultPlayListManager CreateManager() => new(new TestDepository(), new NoopNotificationHub());
}
