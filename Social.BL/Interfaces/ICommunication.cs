using CommunityNetwork.Common.Inerfaces;
using CommunityNetwork.Common.Models;
using CommunityNetWork.Common.Enums;
using Social.BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.BL.Interfaces
{
    public interface ICommunication
    {
        void Link<TNode, TLinked>(string nodeId, string linkedId, Linkage linkage)
           where TNode : INode
           where TLinked : INode;

        void UnLink<TNode, TLinked>(string nodeId, string linkedId, Linkage linkage) where TNode : INode where TLinked : INode;

        bool IsLinker<TNode, TLinked>(string nodeId, string linkedId, Linkage linkage)
           where TNode : INode where TLinked : INode;

         List<TLinkedBy> GetNodeLinkedBy<TLinkedBy, TLinker>(string linkerId, Linkage linkage)
            where TLinker : MNode
            where TLinkedBy : MNode;

        List<TLinker> GetNodeLinkers<TLinkedBy, TLinker>(string linkedById, Linkage linkage)
            where TLinker : MNode
            where TLinkedBy : MNode;

        List<MNode> GetNotLinked<TNode,TNotLinked>(string nodeId, Linkage linkage)
            where TNode : INode where TNotLinked : MNode;
        


        List<TLinkedByLinkedBy> GetNodeLinkedByLinkedByWithLinkersCount<TLinkedByLinkedBy, TLinkedBy, TLinker>(
            string linkerId,
            Linkage linkageOfLinkedBy,
            Linkage linkageOfLinkedByLinkedBy,
            Linkage linkageOfLinkersCount)
            where TLinkedByLinkedBy : INode
            where TLinkedBy : INode
            where TLinker : INode;

            
        

        void LinkProfiles(SocialAction socialAction, bool swch = true);
        
        
        void Like<TLikeable>(string likerId, string likeableId,bool swch=true)
            where TLikeable : MNode, ILikeable;
        void UnLike<TLikeable>(string likerId, string likeableId)
            where TLikeable : MNode, ILikeable;

        bool Publish<TPublish>(string profileId, TPublish publish)
            where TPublish : IPost;

        void LinkProfileToPost(SocialAction socialAction, bool swch = true);
    }
}
