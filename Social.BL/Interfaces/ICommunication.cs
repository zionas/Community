using CommunityNetwork.Common.Inerfaces;
using CommunityNetwork.Common.Models;
using CommunityNetWork.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.BL.Interfaces
{
    public interface ICommunication
    {
        void Link<TNode, TLinked>(Guid nodeId, Guid linkedId, Linkage linkage)
           where TNode : INode
           where TLinked : INode;

        void UnLink<TNode, TLinked>(Guid nodeId, Guid linkedId, Linkage linkage) where TNode : INode where TLinked : INode;

        bool IsLinked<TNode, TLinked>(Guid nodeId, Guid linkedId, Linkage linkage)
           where TNode : INode where TLinked : INode;

       List<TLinked>  GetLinkers<TNode, TLinked>(Guid nodeId, Linkage linkage) where TNode : INode where TLinked : INode;
        List<MNode> GetNotLinked<TNode,TNotLinked>(Guid nodeId, Linkage linkage)
            where TNode : INode where TNotLinked : MNode;
        List<TLinked> GetNewLinkers<TNode, TLinked>(Guid nodeId, Linkage linkage, DateTime dateTime)
        where TNode : INode
            where TLinked : INode;


        List<Tuple<TLinkedBy, TLinkedByLinkedBy>> GetNewLinkedByLinkers<TNode, TLinkedBy, TLinkedByLinkedBy>(Guid nodeId, Linkage linkage, Linkage linkageBy, DateTime dateTime)
            where TNode : INode where TLinkedBy : INode where TLinkedByLinkedBy : INode;


        
        void Block(Guid blockerId, Guid blockedId);
        void UnBlock(Guid blockerId, Guid blockedId);
        void Follow(Guid followerId, Guid followedId);
        void Like<TLikeable>(Guid likerId, Guid likeableId)
            where TLikeable : MNode, ILikeable;
        void UnLike<TLikeable>(Guid likerId, Guid likeableId)
            where TLikeable : MNode, ILikeable;
        
    }
}
