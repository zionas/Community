
using System;
using System.Collections.Generic;


namespace CommunityNetwork.Common.Inerfaces
{
    interface IPost:ILikeable,INode
    {
        List<string> Likers { get; set; }


    }
}
