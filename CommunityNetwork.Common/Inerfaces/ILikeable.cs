

using System.Collections.Generic;

namespace CommunityNetwork.Common.Inerfaces
{
    public interface ILikeable:INode
        
    {
        
        List<Profile> Likers();
        
    }
}
