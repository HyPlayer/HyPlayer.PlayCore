using HyPlayer.PlayCore.Abstraction.Models.AudioServiceComponents;
using HyPlayer.PlayCore.Abstraction.Models.Notifications;

namespace HyPlayer.PlayCore.Implementation.AudioGraphService.Abstractions.Notifications
{
    public class AudioGraphMasterTicketChangedNotification : MasterTicketChangedNotification
    {
        public override AudioTicketBase MasterTicket { get; init; }
        public AudioGraphMasterTicketChangedNotification(AudioGraphTicket ticket)
        {
            MasterTicket = ticket;
        }
    }
}
