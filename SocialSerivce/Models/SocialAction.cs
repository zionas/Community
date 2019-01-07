using CommunityNetWork.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialSerivce.Models
{
    public class SocialAction
    {
        
            public Guid FromId { get; set; }
            public Guid ToId { get; set; }
            public string linkage { get; set; }
    }
}
