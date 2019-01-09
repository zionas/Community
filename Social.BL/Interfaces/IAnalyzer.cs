using CommunityNetwork.Common.Inerfaces;
using CommunityNetWork.Common.Enums;

using System;
using System.Collections.Generic;


namespace Social.BL.Interfaces
{
    interface ILinkageAnalyzer<TNode> where TNode:INode
    {
        List<TLinkedBy> GetLinks<TLinkedBy>(string nodeId, Linkage linkage) 
            where TLinkedBy :INode;
        List<TLinkedBy> GetLinked<TLinkedBy>(string nodeId, Linkage linkage) 
            where TLinkedBy : INode;

    }

    
}
