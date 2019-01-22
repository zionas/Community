
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

        public void Link<TNode,TLinked>(string linkedById, string linkerId,Linkage linkage) 
            where TNode:INode 
            where TLinked:INode
        {
            using (IGraph graph = (IGraph)_graphFactory.Create())
            {

                graph.LinkWithParams<TNode, TLinked>(linkedById, linkerId, linkage, new LinkParams());

            }
        }
        
        public void UnLink<TNode, TLinked>(string linkedById, string linkerId, Linkage linkage) where TNode : INode where TLinked : INode
        {
            using (IGraph graph = (IGraph)_graphFactory.Create())
            {
                graph.UnLink<TNode, TLinked>(linkedById, linkerId, linkage);
            }
        }
        
        public bool IsLinker<TNode,TLinked>(string linkedById,string linkerId,Linkage linkage)
            where TNode : INode where TLinked : INode
        {
            using (IGraph graph = (IGraph)_graphFactory.Create())
            {
                return graph.IsLinkerOfLinkedBy<TNode, TLinked>(linkedById, linkerId, linkage);
            }
        }

        public List<TLinker> GetLinkers<TNode,TLinker>(string linkedById, Linkage linkage) where TNode:INode where TLinker:INode
        {
            using (IGraph graph = (IGraph)_graphFactory.Create())
            {
                return graph.GetNodeLinkers<TNode, TLinker>(linkedById, linkage);
            }
        }

        public List<TLinkedBy> GetLinkedBy<TLinker,TLinkedBy>(string linkerId, Linkage linkage) where TLinkedBy : INode where TLinker : INode
        {
            using (IGraph graph = (IGraph)_graphFactory.Create())
            {
                return graph.GetNodeLinkedBy<TLinker, TLinkedBy>(linkerId, linkage);
            }
        }

        public List<TLinked> GetLinks<TNode, TLinked>(string nodeId, Linkage linkage) where TNode : INode where TLinked : INode
        {
            using (IGraph graph = (IGraph)_graphFactory.Create())
            {
                return graph.GetNodeLinkers<TNode, TLinked>(nodeId, linkage);
            }
        }
        public  List<TLinked> GetNewLinkers<TNode, TLinked>(string nodeId, Linkage linkage,DateTime dateTime) 
            where TNode : INode
            where TLinked:INode
        {
            using (IGraph graph = (IGraph)_graphFactory.Create())
            {
                return graph.GetNodeNewLinks<TNode, TLinked>(nodeId, linkage, dateTime);
            }
        }


        public List<TLinkedByLinkedBy> GetLinkedByLinkedBy<TNode, TLinkedBy, TLinkedByLinkedBy>(string nodeId, Linkage linkage, Linkage linkageBy)
            where TNode : INode 
            where TLinkedBy : INode 
            where TLinkedByLinkedBy : INode
        {
            using (IGraph graph = (IGraph)_graphFactory.Create())
            {
                return graph.GetNodeLinkedByLinkedBy<TNode, TLinkedBy, TLinkedByLinkedBy>(nodeId, linkage, linkageBy);
            }
        }

        public  List<Tuple<TLinkedBy, TLinkedByLinkedBy>> GetNewLinkedByLinkers<TNode, TLinkedBy,TLinkedByLinkedBy>(string nodeId, Linkage linkage, Linkage linkageBy, DateTime dateTime)
            where TNode : INode where TLinkedBy : INode where TLinkedByLinkedBy : INode
        {
            using (IGraph graph = (IGraph)_graphFactory.Create())
            {
                return graph.GetNodeNewLinkedByLinkedBy<TNode, TLinkedBy, TLinkedByLinkedBy>(nodeId, linkage, linkageBy, dateTime);
            }
        }


        

        public List<MNode> GetNotLinked<TNode, TNotLinked>(string nodeId, Linkage linkage)
            where TNode : INode where TNotLinked : MNode
        {
            using (IGraph graph = (IGraph)_graphFactory.Create())
            {
                return graph.GetNodeNotLinks<TNode,MNode>(nodeId, linkage);
            }
        }


        public List<TLinker> GetNodeLinkers<TLinkedBy, TLinker>(string linkedById, Linkage linkage)
            where TLinker : MNode
            where TLinkedBy : MNode
        {
            using (IGraph graph = (IGraph)_graphFactory.Create())
            {
                return graph.GetNodeLinkers<TLinkedBy, TLinker>(linkedById, linkage);

            }
        }

        public List<TLinker> GetNodeLinkedBy<TLinkedBy, TLinker>(string linkerId, Linkage linkage)
            where TLinker : MNode
            where TLinkedBy : MNode
        {
            using (IGraph graph = (IGraph)_graphFactory.Create())
            {
                return graph.GetNodeLinkedBy<TLinkedBy, TLinker>(linkerId, linkage);

            }
        }

        public bool Publish<TPublish>(string profileId, TPublish publish)
        where TPublish : IPost
        {
            using (IGraph graph = (IGraph)_graphFactory.Create())
            {
                return graph.Link<TPublish, Profile>(profileId, publish.Id, Linkage.Publish);
            }
        }

        public List<TLinkedByLinkedBy> GetNodeLinkedByLinkedByWithLinkersCount<TLinkedByLinkedBy, TLinkedBy, TLinker>(
            string linkerId,
            Linkage linkageOfLinkedBy,
            Linkage linkageOfLinkedByLinkedBy,
            Linkage linkageOfLinkersCount)
            where TLinkedByLinkedBy : INode
            where TLinkedBy : INode
            where TLinker : INode
            

        {

            using (IGraph graph = (IGraph)_graphFactory.Create())
            {
                var results = graph.GetNodeLinkedByLinkedByResults<TLinkedByLinkedBy, TLinkedBy, TLinker>(linkerId, linkageOfLinkedBy, linkageOfLinkedByLinkedBy);

                return graph.GetNodesWithLinkersCount<TLinkedByLinkedBy, TLinkedBy>(results, linkageOfLinkersCount, "linkedByLinkedBy");
            }
        }
        
        public List<TLinker> GetNodeLinkedByNotLinks<TLinkedBy, TLinker>(string linkerId, Linkage linkage1, Linkage linkage2)
            where TLinker : MNode
            where TLinkedBy : MNode
        {
            using (IGraph graph = (IGraph)_graphFactory.Create())
            {
                var results = graph.GetNodeLinkedByResults<TLinkedBy, TLinker>(linkerId, linkage1);
                return graph.GetNodeNotLinks<TLinkedBy, TLinker>(results, "linker as node1,linkedBy as node2", linkerId, linkage2);

            }
        }
        public void LinkTo(SocialAction socialAction,bool swch=true)
        {
            string linkerId = socialAction.FromId;
            string linkedById = socialAction.ToId;
            Linkage linkage = (Linkage)Enum.Parse(typeof(Linkage),socialAction.linkage);
                if (swch)

                 Link<Profile, Profile>(linkedById, linkerId, linkage);
            
            else
                UnLink<Profile, Profile>(linkedById, linkerId, linkage);

        }

        public void LinkProfileToPost(SocialAction socialAction, bool swch = true)
        {
            string linkerId = socialAction.FromId;
            string linkedId = socialAction.ToId;
            Linkage linkage = (Linkage)Enum.Parse(typeof(Linkage), socialAction.linkage);
            if (swch)
                Link<Post, Profile>(linkedId, linkerId, linkage);

            else
                UnLink<Post, Profile>(linkerId, linkedId, linkage);

        }

        public void LinkIf<TLinkedBy,TLinker>(SocialAction socialAction, bool swch = true)
            where TLinkedBy:INode
            where TLinker:INode
        {
            string linkerId = socialAction.FromId;
            string linkedId = socialAction.ToId;
            Linkage linkage = (Linkage)Enum.Parse(typeof(Linkage), socialAction.linkage);
            if (swch)
                Link<TLinkedBy, TLinker>(linkedId, linkerId, linkage);

            else
                UnLink<TLinkedBy, TLinker>(linkerId, linkedId, linkage);

        }





        public void Like<TLikeable>(string likerId,
                                    string likeableId,
                                    bool swch=true)
            where TLikeable : MNode, ILikeable
        {
            if(swch)
                Link<Profile, TLikeable>(likerId, likeableId, Linkage.Like);
            else
                UnLink<Profile, TLikeable>(likerId, likeableId, Linkage.Like);
        }


        public void UnLike<TLikeable>(string likerId, string likeableId)
            where TLikeable : MNode, ILikeable
        {
            UnLink<Profile, TLikeable>(likerId, likeableId, Linkage.Like);
        }

    }
}
