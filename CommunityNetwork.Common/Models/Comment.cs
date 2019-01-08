using CommunityNetwork.Common.Inerfaces;
using System;
using System.Collections.Generic;

namespace CommunityNetwork.Common.Models
{
    public class Comment:MNode, IPost
    {
        public string Content { get; set; }
        public DateTime CommentTime { get; set; }
        public List<string> Likers { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Save()
        {

        }

        public void GetComments()
        {

        }

        public void GetLikers()
        {

        }

        public void DoComment()
        {

        }

        public void DoLike()
        {

        }

        public void Like()
        {
            throw new NotImplementedException();
        }

        List<Profile> ILikeable.Likers()
        {
            throw new NotImplementedException();
        }
    }
}
