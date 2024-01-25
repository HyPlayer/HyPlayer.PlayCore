using HyPlayer.PlayCore.Abstraction.Models.AudioServiceComponents;
using HyPlayer.PlayCore.Abstraction.Models.Resources;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.Audio;
using Windows.Media.Core;
using Windows.Storage;

namespace HyPlayer.PlayCore.Implementation.AudioGraphService.Abstractions
{
    public class AudioGraphTicket : AudioTicketBase, IDisposable
    {
        public MediaSourceAudioInputNode PlaybackMediaSourceInputNode { get; internal set; }
        public MediaSource PlaybackMediaSource { get; internal set; }
        private bool disposedValue;

        public static async Task<AudioGraphTicket> CreateAudioGraphTicket(MusicResourceBase musicResource, AudioGraph audioGraph)
        {
            var targetUri = new Uri(musicResource.Url);
            MediaSource mediaSource;
            if (targetUri.Host == string.Empty)
            {
                StorageFile storageFile = await StorageFile.GetFileFromPathAsync(musicResource.Url);
                mediaSource = MediaSource.CreateFromStorageFile(storageFile);
            }
            else
            {
                mediaSource = MediaSource.CreateFromUri(targetUri);
            }
            var nodeResult = await audioGraph.CreateMediaSourceAudioInputNodeAsync(mediaSource);
            if (nodeResult.Status != MediaSourceAudioInputNodeCreationStatus.Success)
            {
                throw nodeResult.ExtendedError;
            }
            return new AudioGraphTicket(nodeResult.Node, mediaSource)
            {
                AudioServiceId = "com.storyteller.audiograph.chopin",
                MusicResource = musicResource,
                Status = AudioTicketStatus.None,
            };
        }
        public void ThrowExceptionIfDisposed()
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(typeof(AudioGraphTicket).Name);
            }
        }
        public void Start()
        {
            if (Status != AudioTicketStatus.Playing)
            {
                PlaybackMediaSourceInputNode.Start();
                Status = AudioTicketStatus.Playing;
            }
        }
        public void Stop()
        {
            if (Status != AudioTicketStatus.Stopped)
            {
                PlaybackMediaSourceInputNode.Stop();
                Status = AudioTicketStatus.Stopped;
            }
        }
        public void RemoveAllOutputConnections()
        {
            var connections = PlaybackMediaSourceInputNode.OutgoingConnections.ToList();
            foreach (var connection in connections)
            {
                PlaybackMediaSourceInputNode?.RemoveOutgoingConnection(connection.Destination);
            }
        }
        public async Task ReplaceAudioGraph(AudioGraph audioGraph)
        {
            var previousMediaSourceNode = PlaybackMediaSourceInputNode;
            var creationResult = await audioGraph.CreateMediaSourceAudioInputNodeAsync(PlaybackMediaSource);
            if (creationResult.Status != MediaSourceAudioInputNodeCreationStatus.Success)
            {
                throw creationResult.ExtendedError;
            }
            var newMediaSourceNode = creationResult.Node;
            newMediaSourceNode.Seek(previousMediaSourceNode.Position);
            previousMediaSourceNode.Dispose();
            PlaybackMediaSourceInputNode = newMediaSourceNode;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    PlaybackMediaSourceInputNode?.Stop();
                    RemoveAllOutputConnections();
                    PlaybackMediaSourceInputNode?.Dispose();
                    PlaybackMediaSource?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        private AudioGraphTicket(MediaSourceAudioInputNode playbackMediaSourceInputNode, MediaSource playbackMediaSource)
        {
            PlaybackMediaSourceInputNode = playbackMediaSourceInputNode;
            PlaybackMediaSource = playbackMediaSource;
        }
    }
}
