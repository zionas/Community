
using Amazon.DynamoDBv2.DataModel;
using CommunityNetwork.Common.Inerfaces;
using CommunityNetwork.Common.Models;
using System;

namespace CommunityNetwork.Common
{
    public class Profile : MNode, ISocial, IAuthor,IRecommendable
    {
        [DynamoDBHashKey]
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string WorkPlace { get; set; }
        public DateTime BirthDate { get; set; }
        public string Address { get; set; }



        public Profile()
        {
            
        }

        
        /*public List<string> GetBlocked()
        {
            // neo4j 
            return null;
        }

        public void Block(string profileName)
        {
            _com.Link<Profile, Profile>(Id, profileName, Linkage.Block);
        }
         

        

        public void UnBlock(string profileId)
        {
            _com.UnLink<Profile, Profile>(Id, profileId, Linkage.Like);

        }
        

        public void Follow(string profileId)
        {
            _com.Link<Profile, Profile>(Id, profileId, Linkage.Follow);
        }
        

        public void DoPost(Post post)
        {
            _publisher.Publish(Id, post);
        }
        

        public void DoComment(Comment comment,string commentedId)
        {
            _publisher.Comment<Comment,Post>(Id,comment,commentedId);
        }
        

        public void Like<TLikeable>(string likeableId)
            where TLikeable : Node, ILikeable
        {
            _com.Link<Profile, TLikeable>(Id, likeableId, Linkage.Like);
        }
        

        public void UnLike<TLikeable>(string likeableId)
            where TLikeable : Node, ILikeable
        {
            _com.UnLink<Profile, TLikeable>(Id, likeableId, Linkage.Like);
        }

        public void Block<TSocial>(string socialId )
            where TSocial: Node,ISocial
        {
            _com.Link<Profile, TSocial>(Id, socialId, Linkage.Block);
        }


        public void UnBlock<TSocial>(string socialId)
            where TSocial : Node, ISocial
        {
            _com.UnLink<Profile, TSocial>(Id, socialId, Linkage.Block);
        }

        public void Follow<TSocial>(string socialId)
            where TSocial : Node, ISocial
        {
            _com.Link<Profile, TSocial>(Id, socialId, Linkage.Follow);
        }
        */
    }
}
