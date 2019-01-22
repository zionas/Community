using CommunityNetwork.Common;
using CommunityNetwork.Common.Inerfaces;
using CommunityNetwork.Common.Models;
using CommunityNetWork.Common.Enums;
using CommunityNetWork.Dal;
using CommunityNetWork.Dal.Interfaces;
using Social.BL.Interfaces;
using System;
using System.Collections.Generic;

namespace Social.BL.Models
{
    public class Publisher:IPublisher
    {
        IGraphFactory _graphFactory;
        public Publisher(IGraphFactory graphFactory)
        {
            _graphFactory = graphFactory;
        }
        public TPublish Publish<TPublish>(string profileId, TPublish publish)
            where TPublish:IPost
        {
            using (IGraph graph = (IGraph)_graphFactory.Create())
            {
                return graph.CreateAndLinkWithParams<TPublish,Profile>(profileId, publish, Linkage.Publish, new LinkParams());
            }
        }
        
        
        public Comment Comment<TCommentable>(string profileId,Comment comment,string commentableId)
            where TCommentable:IPost
        {
            Publish(profileId, comment);
            using (IGraph graph = (IGraph)_graphFactory.Create())
            {
                 graph.Link<TCommentable, Comment>(commentableId, comment.Id,Linkage.Comment);
                return comment;
            }
        }

        public Profile GetPublisher<TPostable>(string commentableId)
           where TPostable:IPost
        {
            
            using (IGraph graph = (IGraph)_graphFactory.Create())
            {
                return graph.GetNodeLinkers<TPostable, Profile>(commentableId, Linkage.Publish)[0];
                
            }
        }

        public void RecommendAll<TRecommendable>(Dictionary<string,List<string>> recommended)
            where TRecommendable:IRecommendable
        {
            using (IGraph graph = (IGraph)_graphFactory.Create())
            {
                foreach(string profileId in recommended.Keys)
                {
                    recommended[profileId].ForEach(
                                    recommendableId => graph.Link<Profile, TRecommendable>(recommendableId, profileId, Linkage.Recommended));
                }
                    
                
            }
        }
    }
}
