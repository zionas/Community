using CommunityNetwork.Common.Models;
using CommunityNetWork.Common.Enums;
using CommunityNetWork.Dal;

using System;


namespace Social.BL.Models
{
    public class Publisher
    {

        public bool Publish<TNode>(Guid nodeId, TNode publish) where TNode : MNode
        {
            using (Neo4jConnector neo4j = new Neo4jConnector())
            {
                return neo4j.CreateAndLinkWithParams(nodeId, publish, Linkage.Publish, new LinkParams());
            }
        }
        /**********************************/
        public bool Comment<TNode,TLinked>(Guid profileId,TNode comment,Guid postId) where TNode : MNode  where TLinked:MNode
        {
            Publish(profileId, comment);
            using (Neo4jConnector neo4j = new Neo4jConnector())
            {
                return neo4j.Link<TNode,TLinked>(comment.Id, postId,Linkage.Comment);
            }
        }


    }
}
