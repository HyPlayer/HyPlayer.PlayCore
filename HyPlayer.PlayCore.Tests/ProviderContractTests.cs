using HyPlayer.PlayCore.Abstraction;
using HyPlayer.PlayCore.Abstraction.Interfaces.Provider;
using HyPlayer.PlayCore.Abstraction.Models;
using HyPlayer.PlayCore.Abstraction.Models.Containers;
using HyPlayer.PlayCore.Abstraction.Models.Resources;
using HyPlayer.PlayCore.Abstraction.Models.SingleItems;

namespace HyPlayer.PlayCore.Tests;

public class ProviderContractTests
{
    [Test]
    public async Task AuthAndQrContracts_ExposeProviderOwnedSessionWithoutProviderSpecificTypes()
    {
        var provider = new ContractProvider();

        var session = await provider.LoginAsync("account", "secret");
        var challenge = await provider.CreateQrLoginChallengeAsync();
        var qrState = await provider.GetQrLoginStateAsync(challenge.ChallengeId);
        var exportedSession = await provider.ExportSessionAsync();

        TestAssert.Ensure(session.IsAuthenticated, "Login should return provider-neutral session state.");
        TestAssert.Ensure(challenge.ChallengeId == "challenge" && challenge.Uri == new Uri("https://provider.test/qr"), "QR login should expose a neutral challenge id and URI.");
        TestAssert.Ensure(qrState.Status == ProviderQrLoginStatus.Authorized && qrState.SessionInfo?.IsAuthenticated == true, "QR login polling should expose neutral status and session info.");
        TestAssert.Ensure(exportedSession["session"] == "value", "Session export should use provider-neutral key/value data.");
    }

    [Test]
    public async Task ContainerCommentAndSearchContracts_ReturnProviderNeutralModels()
    {
        var provider = new ContractProvider();

        var container = await provider.CreateContainerAsync("Created", true);
        var page = await provider.GetContainerItemsPageAsync("playlist-1", 0, 10);
        var comments = await provider.GetCommentsAsync("song-1", "song", 0, 20);
        var suggestions = await provider.GetSearchSuggestionsAsync("hy");

        TestAssert.Ensure(container is TestLinerContainer, "Container management should return ContainerBase instances.");
        TestAssert.Ensure(page.Items.Single().ActualId == "song-1" && page.HasMore == false, "Container paging should return provider-neutral providable items.");
        TestAssert.Ensure(comments.Items.Single().Content == "comment", "Comment operations should return CommentBase items.");
        TestAssert.Ensure(suggestions is TestLinerContainer, "Search suggestions should be grouped in a ContainerBase.");
    }

    [Test]
    public async Task UploadRichMediaAndListenTogetherContracts_ReturnProviderNeutralModels()
    {
        var provider = new ContractProvider();

        var uploaded = await provider.UploadCloudLibraryItemAsync(new ContractResource(), new Dictionary<string, string> { ["title"] = "Song" });
        var richMedia = await provider.GetRichMediaAsync("video-1", "video");
        var resource = await provider.GetRichMediaResourceAsync("video-1", "video");
        var feed = await provider.GetRichMediaFeedAsync("video", 0, 10);
        var roomId = await provider.CreateListenTogetherRoomAsync([new TestSong { Name = "Song", ActualId = "song-1" }]);

        TestAssert.Ensure(uploaded.ActualId == "cloud-uploaded", "Cloud upload should return cloud library models.");
        TestAssert.Ensure(richMedia?.ActualId == "video-1" && resource is ContractResource && feed.Items.Single().ActualId == "video-1", "Rich media operations should return neutral rich media and resource models.");
        TestAssert.Ensure(roomId == "room" && await provider.CanJoinListenTogetherRoomAsync(roomId), "Listen-together operations should use neutral room ids and queues.");
    }

    private sealed class ContractProvider : ProviderBase,
                                           IAuthenticationProvidable,
                                           IQrAuthenticationProvidable,
                                            IContainerManagementProvidable,
                                            IContainerPageProvidable,
                                            ICommentProvidable,
                                            ISearchSuggestionProvidable,
                                            IProvidableItemDynamicMetadataProvidable,
                                             ICloudUploadProvidable,
                                             IRichMediaProvidable,
                                             IListenTogetherProvidable
    {
        public override string Name => "Contract Provider";
        public override string Id => "provider.contract";
        public override List<ProvidableTypeId> ProvidableTypeIds => [];

        public Task<ProviderSessionInfo> LoginAsync(string accountId, string secret, CancellationToken ctk = new())
            => Task.FromResult(new ProviderSessionInfo { IsAuthenticated = true, UserId = accountId, DisplayName = accountId });

        public Task LogoutAsync(CancellationToken ctk = new()) => Task.CompletedTask;

        public Task<ProviderSessionInfo> GetSessionInfoAsync(CancellationToken ctk = new())
            => Task.FromResult(new ProviderSessionInfo { IsAuthenticated = true, UserId = "account" });

        public Task ImportSessionAsync(IReadOnlyDictionary<string, string> sessionValues, CancellationToken ctk = new()) => Task.CompletedTask;

        public Task<IReadOnlyDictionary<string, string>> ExportSessionAsync(CancellationToken ctk = new())
            => Task.FromResult<IReadOnlyDictionary<string, string>>(new Dictionary<string, string> { ["session"] = "value" });

        public Task AnnounceDeviceAsync(string deviceName, CancellationToken ctk = new()) => Task.CompletedTask;

        public Task<ProviderQrLoginChallenge> CreateQrLoginChallengeAsync(CancellationToken ctk = new())
            => Task.FromResult(new ProviderQrLoginChallenge { ChallengeId = "challenge", Uri = new Uri("https://provider.test/qr") });

        public Task<ProviderQrLoginState> GetQrLoginStateAsync(string challengeId, CancellationToken ctk = new())
            => Task.FromResult(new ProviderQrLoginState { Status = ProviderQrLoginStatus.Authorized, SessionInfo = new ProviderSessionInfo { IsAuthenticated = true } });

        public Task<ContainerBase> CreateContainerAsync(string name, bool isPrivate, CancellationToken ctk = new())
            => Task.FromResult<ContainerBase>(new TestLinerContainer([]) { Name = name, ActualId = "created" });

        public Task DeleteContainerAsync(string containerId, CancellationToken ctk = new()) => Task.CompletedTask;

        public Task SetContainerPrivacyAsync(string containerId, bool isPrivate, CancellationToken ctk = new()) => Task.CompletedTask;

        public Task<ProviderPageResult<ProvidableItemBase>> GetContainerItemsPageAsync(string containerId, int offset, int count, CancellationToken ctk = new())
            => Task.FromResult(new ProviderPageResult<ProvidableItemBase> { Items = [CreateSong()], HasMore = false });

        public Task<ContainerBase?> GetCommentContainerAsync(CancellationToken ctk = new())
            => Task.FromResult<ContainerBase?>(new TestLinerContainer([]) { Name = "Comments", ActualId = "comments" });

        public Task<ProviderPageResult<CommentBase>> GetCommentsAsync(string itemId, string typeId, int offset, int count, int sortType = 1, CancellationToken ctk = new())
            => Task.FromResult(new ProviderPageResult<CommentBase> { Items = [new ContractComment { Name = "Comment", ActualId = "comment-1", Content = "comment" }], HasMore = false });

        public Task<ProviderPageResult<CommentBase>> GetThreadedCommentsAsync(string itemId, string typeId, string commentId, int offset, int count, CancellationToken ctk = new())
            => GetCommentsAsync(itemId, typeId, offset, count, ctk: ctk);

        public Task<CommentBase?> PostCommentAsync(string itemId, string typeId, string content, string? replyToCommentId = null, CancellationToken ctk = new())
            => Task.FromResult<CommentBase?>(new ContractComment { Name = "Created", ActualId = "comment-created", Content = content });

        public Task SetCommentLikeStateAsync(string itemId, string typeId, string commentId, bool like, CancellationToken ctk = new()) => Task.CompletedTask;

        public Task<ContainerBase> GetSearchSuggestionsAsync(string keyword, CancellationToken ctk = new())
            => Task.FromResult<ContainerBase>(new TestLinerContainer([CreateSong()]) { Name = keyword, ActualId = "suggestions" });

        public Task<ProvidableItemDynamicMetadata> GetDynamicMetadataAsync(string itemId, string typeId, CancellationToken ctk = new())
            => Task.FromResult(new ProvidableItemDynamicMetadata { CommentCount = 1, IsLiked = true });

        public Task<CloudLibraryItemBase> UploadCloudLibraryItemAsync(ResourceBase resource, IReadOnlyDictionary<string, string> metadata, CancellationToken ctk = new())
            => Task.FromResult<CloudLibraryItemBase>(new ContractCloudItem { Name = metadata["title"], ActualId = "cloud-uploaded" });

        public Task<RichMediaBase?> GetRichMediaAsync(string mediaId, string typeId, CancellationToken ctk = new())
            => Task.FromResult<RichMediaBase?>(new ContractRichMedia { Name = "Video", ActualId = mediaId });

        public Task<ResourceBase?> GetRichMediaResourceAsync(string mediaId, string typeId, string? qualityId = null, CancellationToken ctk = new())
            => Task.FromResult<ResourceBase?>(new ContractResource { ResourceName = mediaId });

        public Task<ProviderPageResult<RichMediaBase>> GetRichMediaFeedAsync(string? typeId, int offset, int count, CancellationToken ctk = new())
            => Task.FromResult(new ProviderPageResult<RichMediaBase> { Items = [new ContractRichMedia { Name = "Video", ActualId = "video-1" }], HasMore = false });

        public Task<string> CreateListenTogetherRoomAsync(List<SingleSongBase> queue, CancellationToken ctk = new())
            => Task.FromResult("room");

        public Task<bool> CanJoinListenTogetherRoomAsync(string roomId, CancellationToken ctk = new())
            => Task.FromResult(true);

        public Task SendListenTogetherPlaybackCommandAsync(string roomId, ProviderListenTogetherPlaybackCommand command, CancellationToken ctk = new()) => Task.CompletedTask;

        public Task ReportListenTogetherQueueAsync(string roomId, ProviderListenTogetherQueueReport report, CancellationToken ctk = new()) => Task.CompletedTask;

        public Task<ProviderListenTogetherStatus?> GetListenTogetherStatusAsync(string roomId, CancellationToken ctk = new())
            => Task.FromResult<ProviderListenTogetherStatus?>(new ProviderListenTogetherStatus { IsInRoom = true, RoomId = roomId });

        private static TestSong CreateSong() => new() { Name = "Song", ActualId = "song-1" };
    }

    private sealed class ContractComment : CommentBase
    {
        public override string ProviderId => "provider.contract";
        public override string TypeId => "comment";
    }

    private sealed class ContractCloudItem : CloudLibraryItemBase
    {
        public override string ProviderId => "provider.contract";
        public override string TypeId => "cloud";
    }

    private sealed class ContractRichMedia : RichMediaBase
    {
        public override string ProviderId => "provider.contract";
        public override string TypeId => "video";
    }

    private sealed class ContractUser : ProvidableItemBase
    {
        public override string ProviderId => "provider.contract";
        public override string TypeId => "user";
    }

    private sealed class ContractResource : ResourceBase
    {
        public override ResourceType Type => ResourceType.Binary;
        public override Task<ResourceResultBase> GetResourceAsync(ResourceQualityTag? qualityTag = null, CancellationToken ctk = new())
            => Task.FromResult<ResourceResultBase>(new TestResourceResult { ResourceStatus = ResourceStatus.Success, ExternalException = null });
    }
}

