using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommunityNetwork.Common.Models
{
    public class SocialAction
    {
        
            public string FromId { get; set; }
            public string ToId { get; set; }
            public string linkage { get; set; }
            public bool Switcher { get; set; }
    }
}
