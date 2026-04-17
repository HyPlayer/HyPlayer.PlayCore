using HyPlayer.PlayCore.Abstraction.Models.AudioServiceComponents;

namespace HyPlayer.PlayCore.Abstraction.Models.Notifications
{
    public abstract class MasterTicketChangedNotification
    {
        public abstract AudioTicketBase MasterTicket { get; init; }
    }
}
