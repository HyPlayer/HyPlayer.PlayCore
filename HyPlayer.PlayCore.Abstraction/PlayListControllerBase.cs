using System.Collections.ObjectModel;
using HyPlayer.PlayCore.Abstraction.Models.Songs;

namespace HyPlayer.PlayCore.Abstraction;

public abstract class PlayListControllerBase
{
    public abstract Task<List<SingleSongBase>> GetPlayList();
    public abstract Task<int> GetCurrentIndex();
    public abstract Task LoadSongContainer(List<SingleSongBase> songs);
    public abstract Task InsertSong(SingleSongBase song, int index);
    public abstract Task InsertSongRange(ReadOnlyCollection<SingleSongBase> song, int index);
    public abstract Task RemoveSong(SingleSongBase song);
    public abstract Task ClearSongs();
    public abstract Task RemoveSongRange(ReadOnlyCollection<SingleSongBase> song);
    public abstract Task ChangePlayingTo(SingleSongBase song);
    public abstract Task<SingleSongBase> GetNextSong();
    public abstract Task<SingleSongBase> MoveNext();
    public abstract Task<SingleSongBase> MovePrevious();
}