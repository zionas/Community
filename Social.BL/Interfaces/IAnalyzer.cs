using CommunityNetWork.Dal;
using CommunityNetWork.Dal.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.BL.Interfaces
{
    interface ILinkageAnalyzer<TNode> where TNode:INode
    {
        List<TLinkedBy> GetLinks<TLinkedBy>(Guid nodeId, Linkage linkage) where TLinkedBy : INode;
        List<TLinkedBy> GetLinked<TLinkedBy>(Guid nodeId, Linkage linkage) where TLinkedBy : INode;

    }

    
}
