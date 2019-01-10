using CommunityNetwork.Common.Inerfaces;
using CommunityNetWork.Common.Enums;
using CommunityNetWork.Dal;
using Social.BL.Interfaces;
using System;
using System.Collections.Generic;


namespace Social.BL
{
    class LinkageAnalyzer<TNode> : ILinkageAnalyzer<TNode> where TNode:INode
    {
        public List<TLinkedBy> GetLinked<TLinkedBy>(string nodeId,Linkage linkage)
            
            where TLinkedBy : INode

        {
            using (Neo4jConnector neo4j = new Neo4jConnector())
            {
                var list = neo4j.GetNodeLinksBy<TNode, TLinkedBy>(nodeId, linkage);
                return list;
            }
        }

        

        public List<TLinked> GetLinks<TLinked>(string nodeId,Linkage linkage)
            
            where TLinked:INode
            
        {
            using (Neo4jConnector neo4j = new Neo4jConnector())
            {
                var list = neo4j.GetNodeLinkers<TNode, TLinked>(nodeId, linkage);
                return list;
            }
            
        }

        
    }
}
