namespace HyPlayer.PlayCore.Implementation.AudioGraphService.Abstractions.Notifications
{
    public class AudioTicketReachesEndNotification
    {
        public AudioGraphTicket AudioGraphTicket { get; init; }
        public AudioTicketReachesEndNotification(AudioGraphTicket ticket)
        {
            AudioGraphTicket = ticket;
        }
    }
}
