using CommunityNetWork.Dal;
using CommunityNetWork.Dal.Enums;
using Social.BL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.BL
{
    class LinkageAnalyzer<TNode> : ILinkageAnalyzer<TNode> where TNode:INode
    {
        public List<TLinkedBy> GetLinked<TLinkedBy>(Guid nodeId,Linkage linkage)
            
            where TLinkedBy : INode

        {
            using (Neo4jConnector neo4j = new Neo4jConnector())
            {
                var list = neo4j.GetNodeLinksBy<TNode, TLinkedBy>(nodeId, linkage);
                return list;
            }
        }

        

        public List<TLinked> GetLinks<TLinked>(Guid nodeId,Linkage linkage)
            
            where TLinked:INode
            
        {
            using (Neo4jConnector neo4j = new Neo4jConnector())
            {
                var list = neo4j.GetNodeLinks<TNode, TLinked>(nodeId, linkage);
                return list;
            }
            
        }

        
    }
}
