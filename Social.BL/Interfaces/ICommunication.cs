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

        bool IsLinked<TNode, TLinked>(string nodeId, string linkedId, Linkage linkage)
           where TNode : INode where TLinked : INode;

       List<TLinked>  GetLinkers<TNode, TLinked>(string nodeId, Linkage linkage) where TNode : INode where TLinked : INode;
        List<MNode> GetNotLinked<TNode,TNotLinked>(string nodeId, Linkage linkage)
            where TNode : INode where TNotLinked : MNode;
        List<TLinked> GetNewLinkers<TNode, TLinked>(string nodeId, Linkage linkage, DateTime dateTime)
        where TNode : INode
            where TLinked : INode;


        List<Tuple<TLinkedBy, TLinkedByLinkedBy>> GetNewLinkedByLinkers<TNode, TLinkedBy, TLinkedByLinkedBy>(string nodeId, Linkage linkage, Linkage linkageBy, DateTime dateTime)
            where TNode : INode where TLinkedBy : INode where TLinkedByLinkedBy : INode;


        
        void LinkProfiles(SocialAction socialAction, bool swch = true);
        
        
        void Like<TLikeable>(string likerId, string likeableId,bool swch=true)
            where TLikeable : MNode, ILikeable;
        void UnLike<TLikeable>(string likerId, string likeableId)
            where TLikeable : MNode, ILikeable;
        
    }
}
