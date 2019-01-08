
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
    public  class Communication:ICommunication
    {


        IGraphFactory _graphFactory;
        public Communication(IGraphFactory graphFactory)
        {
            _graphFactory = graphFactory;
        }
        public void Link<TNode,TLinked>(Guid nodeId, Guid linkedId,Linkage linkage) 
            where TNode:INode 
            where TLinked:INode
        {
            using (IGraph graph = (IGraph)_graphFactory.Create())
            {
                graph.LinkWithParams<TNode, TLinked>(nodeId, linkedId, linkage, new LinkParams());
            }
        }
        
        public void UnLink<TNode, TLinked>(Guid nodeId, Guid linkedId, Linkage linkage) where TNode : INode where TLinked : INode
        {
            using (IGraph graph = (IGraph)_graphFactory.Create())
            {
                graph.UnLink<TNode, TLinked>(nodeId, linkedId, linkage);
            }
        }
        
        public bool IsLinked<TNode,TLinked>(Guid nodeId,Guid linkedId,Linkage linkage)
            where TNode : INode where TLinked : INode
        {
            using (IGraph graph = (IGraph)_graphFactory.Create())
            {
                return graph.IsLinked<TNode, TLinked>(nodeId, linkedId, linkage);
            }
        }
        public List<TLinked> GetLinkers<TNode,TLinked>(Guid nodeId, Linkage linkage) where TNode:INode where TLinked:INode
        {
            using (IGraph graph = (IGraph)_graphFactory.Create())
            {
                return graph.GetNodeLinks<TNode, TLinked>(nodeId, linkage);
            }
        }
        /*******************************************************/
        public  List<TLinked> GetNewLinkers<TNode, TLinked>(Guid nodeId, Linkage linkage,DateTime dateTime) 
            where TNode : INode
            where TLinked:INode
        {
            using (IGraph graph = (IGraph)_graphFactory.Create())
            {
                return graph.GetNodeNewLinks<TNode, TLinked>(nodeId, linkage, dateTime);
            }
        }
        /*******************************************************/
        public  List<Tuple<TLinkedBy, TLinkedByLinkedBy>> GetNewLinkedByLinkers<TNode, TLinkedBy,TLinkedByLinkedBy>(Guid nodeId, Linkage linkage, Linkage linkageBy, DateTime dateTime)
            where TNode : INode where TLinkedBy : INode where TLinkedByLinkedBy : INode
        {
            using (IGraph graph = (IGraph)_graphFactory.Create())
            {
                return graph.GetNodeNewLinkedByLinkedBy<TNode, TLinkedBy, TLinkedByLinkedBy>(nodeId, linkage, linkageBy, dateTime);
            }
        }
        public List<MNode> GetNotLinked<TNode, TNotLinked>(Guid nodeId, Linkage linkage)
            where TNode : INode where TNotLinked : MNode
        {
            using (IGraph graph = (IGraph)_graphFactory.Create())
            {
                return graph.GetNodeNotLinks<TNode,MNode>(nodeId, linkage);
            }
        }

        
        public void Block(Guid blockerId,Guid blockedId)
        {
            Link<ISocial, ISocial>(blockerId, blockedId, Linkage.Block);
        }


       
        public void UnBlock(Guid blockerId, Guid blockedId)
        {
            
                UnLink<ISocial, ISocial>(blockerId, blockedId, Linkage.Like);

        }
        
        
        public void Follow(Guid followerId, Guid followedId)
        {
            Link<Profile, Profile>(followerId,followedId, Linkage.Follow);
        }
        
        public void Like<TLikeable>(Guid likerId,Guid likeableId)
            where TLikeable : MNode, ILikeable
        {
            Link<Profile, TLikeable>(likerId, likeableId, Linkage.Like);
        }


        public void UnLike<TLikeable>(Guid likerId, Guid likeableId)
            where TLikeable : MNode, ILikeable
        {
            UnLink<Profile, TLikeable>(likerId, likeableId, Linkage.Like);
        }

        
    }
}
