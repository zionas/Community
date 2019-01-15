
using System;

using CommunityNetwork.Common.Enums;
namespace CommunityNetwork.Common.Models
{
    public class Notification
    {
        public string NotifiedUserId { get; set; }
        public DateTime Time { get; set; }
        public NotificationType NotificationType { get; set; }

        
    }
}
