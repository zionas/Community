
using System;

using CommunityNetwork.Common.Enums;
namespace CommunityNetwork.Common.Models
{
    public class Notification
    {
        public string NotifiedUserName { get; set; }
        public DateTime Time { get; set; }
        public NotificationType NotificationType { get; set; }

        
    }
}
