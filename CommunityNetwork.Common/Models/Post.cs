using CommunityNetwork.Common.Enums;
using CommunityNetwork.Common.Inerfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityNetwork.Common.Models
{
    public class Post: MNode,IPost,IRecommendable
    {
        
        public VisibilityPermission VisibilityPermission { get; set; }
        public string ImageSourcePath { get; set; }
        public int Likes { get ; set ; }
        public List<Profile> Likers { get; set; }
        public string Content { get; set; }

        

        public List<Comment> GetComments()
        {
            /// neo4j return count of likes
            return new List<Comment>();
        }

        public void DoLike(Profile profile)
        {
            /// neo4j create connection of 'Like'
        }

        public void Comment(Profile profile)
        {
            /// neo4j create connection of 'Like'
        }

        public void EditPost(string content)
        {
            Content = content;
        }
        

        public List<Profile> GetLikers()
        {
            return new List<Profile>();
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
