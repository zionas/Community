using CommunityNetwork.Common.Inerfaces;
using CommunityNetwork.Common.Models;
using CommunityNetWork.Common.Enums;
using Neo4jClient.Cypher;
using System;
using System.Collections.Generic;


namespace CommunityNetWork.Dal.Interfaces
{
    public interface IGraph: IDBConnector
    {
        List<TNode> Get<TNode>(string propertyName, object value) where TNode : INode;

       TNode Get<TNode>(string id) where TNode : INode;
        List<TNode> Get<TNode>() where TNode : INode;
        TNode Create<TNode>(TNode node) where TNode : INode;
        TNode Put<TNode>(TNode node) where TNode : INode;
        bool Delete<TNode>(string id) where TNode : INode;

        TNew CreateAndLink<TNew, TLinked>(string linkedId, TNew newNode, Linkage linkage)
            where TNew : INode
            where TLinked : INode;
        TNew CreateAndLinkWithParams<TNew, TLinked>(string linkedId, TNew newNode, Linkage linkage, LinkParams linkParams)
            where TNew : INode
            where TLinked : INode;
        bool IsLinker<TLinker, TLinkedBy>(string linkedById, string linkerId, Linkage linkage)
            where TLinker : INode where TLinkedBy : INode;

        bool IsLinkedBy<TNode, TLinkedBy>(string linkedById, string linkerId, Linkage linkage)
            where TNode : INode where TLinkedBy : INode;

         Dictionary<TNode, List<TLinked>> GetNodesLinks<TNode, TLinked>(Linkage linkage)
       where TNode : INode where TLinked : INode;


       

        bool Link<TNode, TLinked>(string nodeId, string linkedId, Linkage linkage) where TNode : INode where TLinked : INode;

        bool UnLink<TNode, TLinked>(string nodeId, string linkedId, Linkage linkage) 
            where TNode : INode where TLinked : INode;

        bool LinkWithParams<TNode, TLinked>(string nodeId, string linkedId, Linkage linkage, LinkParams linkParams) 
            where TNode : INode where TLinked : INode;

         List<LinkedNode> GetNodeLinkersWithData<TLinkedBy, TLinker>(string linkedById, Linkage linkage)
            where TLinkedBy : INode
            where TLinker : INode;
        List<TLinked> GetNodeLinkers<TNode, TLinked>(string nodeId, Linkage linkage)
           where TNode : INode
           where TLinked : INode;

         ICypherFluentQuery GetNodeLinkedByResults<TLinker, TLinkedBy>(string linkerId, Linkage linkage
            )
            where TLinkedBy : INode
            where TLinker : INode;

        List<TLikedBy> GetNodeLinkedBy<TLinker, TLikedBy>(string linkerId, Linkage linkage)
            where TLikedBy : INode
            where TLinker : INode;

        List<TNotLinked> GetNodeNotLinks<TNode, TNotLinked>(string nodeId, Linkage linkage)
           where TNode : INode
           where TNotLinked : INode;

         List<TNotLinked> GetNodeNotLinks<TNode, TNotLinked>(
            ICypherFluentQuery results,
            string with,
            string nodeId,
            Linkage linkage
            )
            where TNode : INode
            where TNotLinked : INode;

        
       List<TLinked> GetNodeNewLinks<TNode, TLinked>(string nodeId, Linkage linkage, DateTime dateTime) where TNode : INode;

       
        List<TLinkedByLinkedBy> GetNodeLinkedByLinkedBy<TNode, TLinkedBy, TLinkedByLinkedBy>(string nodeId, Linkage linkage, Linkage linkageBy)
           where TNode : INode where TLinkedBy : INode where TLinkedByLinkedBy : INode;

        List<MNode> GetNodeLinkedByNotLinkedBy<TNode, TLinkedBy, TLinkedByLinkedBy>(string nodeId, Linkage linkage, Linkage linkageBy)
            where TNode : INode where TLinkedBy : INode where TLinkedByLinkedBy : INode;


        List<Tuple<TLinkedBy, TLinkedByLinkedBy>> GetNodeNewLinkedByLinkedBy<TNode, TLinkedBy, TLinkedByLinkedBy>(string nodeId, Linkage linkage, Linkage linkageBy, DateTime dateTime) where TNode : INode where TLinkedBy : INode where TLinkedByLinkedBy : INode;

               

       List<Tuple<TNode, List<TLinkedBy>>> GetNodesLinksBy<TNode, TLinkedBy>(string nodeId,
                                                                                Linkage linkage)
        where TNode : INode where TLinkedBy : INode;


    }
}
