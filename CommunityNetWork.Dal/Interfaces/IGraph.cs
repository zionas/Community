using CommunityNetwork.Common.Inerfaces;
using CommunityNetwork.Common.Models;
using CommunityNetWork.Common.Enums;

using System;
using System.Collections.Generic;


namespace CommunityNetWork.Dal.Interfaces
{
    public interface IGraph:IDBConnector
    {
        TNode Get<TNode>(string id) where TNode : INode;
        TNode Create<TNode>(TNode node) where TNode : INode;
        void Put(INode node);
        bool Delete<TNode>(string id) where TNode : INode;

        bool CreateAndLink<TNode, TLinked>(string nodeId, TLinked linked, Linkage linkage)
            where TNode : INode
            where TLinked : INode;

        bool IsLinked<TNode, TLinked>(string nodeId, string linkedId, Linkage linkage)
            where TNode : INode where TLinked : INode;

        bool IsLinkedBy<TNode, TLinkedBy>(string nodeId, string linkedId, Linkage linkage)
            where TNode : INode where TLinkedBy : INode;

        List<LinkedNode> GetLinksBy<TNode, TLinked>(MNode linkedBy, Linkage linkage)
            where TNode : INode
            where TLinked : INode;
        
        bool CreateAndLinkWithParams<TNode>(string nodeId, TNode linked, Linkage linkage, LinkParams linkParams) where TNode : INode;

        bool Link<TNode, TLinked>(string nodeId, string linkedId, Linkage linkage) where TNode : INode where TLinked : INode;

        bool UnLink<TNode, TLinked>(string nodeId, string linkedId, Linkage linkage) 
            where TNode : INode where TLinked : INode;

        bool LinkWithParams<TNode, TLinked>(string nodeId, string linkedId, Linkage linkage, LinkParams linkParams) 
            where TNode : INode where TLinked : INode;

        List<TLinked> GetNodeLinks<TNode, TLinked>(string nodeId, Linkage linkage)
            where TNode : INode
            where TLinked : INode;
         List<MNode> GetNodeNotLinks<TNode, TNotLinked>(string nodeId, Linkage linkage)
            where TNode : INode
            where TNotLinked : INode;

        List<TLinked> GetNodeNewLinks<TNode, TLinked>(string nodeId, Linkage linkage, DateTime dateTime) where TNode : INode;

        List<TLinkedBy> GetNodeLinksBy<TNode, TLinkedBy>(string nodeId, Linkage linkage) 
            where TNode : INode
            where TLinkedBy:INode;

        List<TLinkedByLinkedBy> GetNodeLinkedByLinkedBy<TNode, TLinkedBy, TLinkedByLinkedBy>(string nodeId, Linkage linkage, Linkage linkageBy)
           where TNode : INode where TLinkedBy : INode where TLinkedByLinkedBy : INode;

        List<MNode> GetNodeLinkedByNotLinkedBy<TNode, TLinkedBy, TLinkedByLinkedBy>(string nodeId, Linkage linkage, Linkage linkageBy)
            where TNode : INode where TLinkedBy : INode where TLinkedByLinkedBy : INode;


        List<Tuple<TLinkedBy, TLinkedByLinkedBy>> GetNodeNewLinkedByLinkedBy<TNode, TLinkedBy, TLinkedByLinkedBy>(string nodeId, Linkage linkage, Linkage linkageBy, DateTime dateTime) where TNode : INode where TLinkedBy : INode where TLinkedByLinkedBy : INode;

        Dictionary<TNode, List<TLinked>> GetNodesLinks<TNode, TLinked>(Linkage linkage)
        where TNode : INode where TLinked : INode;

        List<Tuple<TNode, List<TLinkedBy>>> GetNodesLinksBy<TNode, TLinkedBy>(string nodeId,
                                                                                Linkage linkage)
        where TNode : INode where TLinkedBy : INode;


    }
}
