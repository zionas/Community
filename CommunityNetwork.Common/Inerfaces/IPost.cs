
using System;
using System.Collections.Generic;


namespace CommunityNetwork.Common.Inerfaces
{
    public interface IPost:ILikeable,INode
    {
        int Likes { get; set; }


    }
}
