using CommunityNetWork.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityNetwork.Common.Inerfaces
{
    public interface ILikeable:INode
        
    {
        
        List<Profile> Likers();
        
    }
}
