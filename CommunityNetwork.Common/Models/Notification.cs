using CommunityNetwork.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityNetwork.Common.Models
{
    public class Notification
    {
        public string NotifiedUserName { get; set; }
        public DateTime Time { get; set; }
        public NotificationType NotificationType { get; set; }

       
    }
}
