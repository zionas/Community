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
        string GetModelName(string id);

        List<TNode> Get<TNode>(string propertyName, object value) where TNode : INode;

       TNode Get<TNode>(string id) where TNode : INode;

        List<TNode> Get<TNode>() where TNode : INode;

        TNode Create<TNode>(TNode node) where TNode : INode;

        TNode Put<TNode>(TNode node) where TNode : INode;
        bool DeleteAll();
        bool Delete<TNode>() where TNode : INode;
        bool Delete<TNode>(string id) where TNode : INode;

        TNew CreateAndLink<TNew, TLinked>(string linkerId, TNew newNode, Linkage linkage)
            where TNew : INode
            where TLinked : INode;
               
        bool IsLinkerOfLinkedBy<TLinker, TLinkedBy>(string linkedById, string linkerId, Linkage linkage)
            where TLinker : INode where TLinkedBy : INode;

        bool IsLinkedBy<TNode, TLinkedBy>(string linkedById, string linkerId, Linkage linkage)
            where TNode : INode where TLinkedBy : INode;
        

        bool Link<TNode, TLinked>(string nodeId, string linkedId, Linkage linkage) where TNode : INode where TLinked : INode;

        bool UnLink<TNode, TLinked>(string nodeId, string linkedId, Linkage linkage) 
            where TNode : INode where TLinked : INode;

        
         List<TLinked> GetNodeLinkers<TNode, TLinked>(string nodeId, Linkage linkage)
           where TNode : INode
           where TLinked : INode;

         ICypherFluentQuery GetNodeLinkedByQuery<TLinkedBy, TLinker>(string linkerId, Linkage linkage
            )
            where TLinkedBy : INode
            where TLinker : INode;

        List<TLinkedBy> GetNodeLinkedBy<TLinkedBy, TLinker>(string linkerId, Linkage linkage)
            where TLinkedBy : INode
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
        
      
        List<TLinkedByLinkedBy> GetNodeLinkedByLinkedBy<TNode, TLinkedBy, TLinkedByLinkedBy>(string nodeId, Linkage linkage, Linkage linkageBy)
           where TNode : INode where TLinkedBy : INode where TLinkedByLinkedBy : INode;

        List<TLinkedByLinkedBy> GetNodeLinkedByNotLinkedBy<TNode, TLinkedBy, TLinkedByLinkedBy>(string nodeId, Linkage linkage, Linkage linkageBy)
            where TNode : INode where TLinkedBy : INode where TLinkedByLinkedBy : INode;

        ICypherFluentQuery GetNodeLinkedByLinkedByQuery<TLinker, TLinkedBy, TLinkedByLinkedBy>(
            string linkerId,
            Linkage linkage,
            Linkage linkageBy)
            where TLinker : INode
            where TLinkedBy : INode
            where TLinkedByLinkedBy : INode;

        List<TLinkedBy> GetNodesWithLinkersCount<TLinkedBy, TLinker>(ICypherFluentQuery results, Linkage linkage, string tag)
           where TLinkedBy : INode
           where TLinker : INode;

        List<TLinkedBy> GetNodesWithLinkers<TLinkedBy, TLinker>(ICypherFluentQuery results, Linkage linkage, string with)
        where TLinkedBy : INode where TLinker : INode;

       
         

    }
}
