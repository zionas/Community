using CommunityNetwork.Common;
using CommunityNetwork.Common.Inerfaces;
using CommunityNetwork.Common.Models;
using CommunityNetWork.Common.Enums;
using CommunityNetWork.Dal;
using Social.BL.Interfaces;
using System;


namespace Social.BL.Models
{
    public class Publisher:IPublisher
    {

        public TPublish Publish<TPublish>(string profileId, TPublish publish)
            where TPublish:IPost
        {
            using (Neo4jConnector neo4j = new Neo4jConnector())
            {
                return neo4j.CreateAndLinkWithParams<TPublish,Profile>(profileId, publish, Linkage.Publish, new LinkParams());
            }
        }
        /**********************************/
        public Comment Comment<TCommentable>(string profileId,Comment comment,string commentableId)
            where TCommentable:IPost
        {
            Publish(profileId, comment);
            using (Neo4jConnector neo4j = new Neo4jConnector())
            {
                 neo4j.Link<Comment,TCommentable>(commentableId, comment.Id,Linkage.Comment);
                return comment;
            }
        }


    }
}
