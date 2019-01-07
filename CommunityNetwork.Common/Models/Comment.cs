using CommunityNetwork.Common.Inerfaces;
using CommunityNetWork.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityNetwork.Common.Models
{
    public class Comment:Node, IPost
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
