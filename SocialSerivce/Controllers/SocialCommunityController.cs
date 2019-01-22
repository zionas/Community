using CommunityNetwork.Common;
using CommunityNetwork.Common.Models;
using CommunityNetWork.Common.Enums;
using Social.BL.Interfaces;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace SocialSerivce.Controllers
{
    [RoutePrefix("api/SocialCommunity")]
    public class SocialCommunityController : ApiController
    {
        ICommunication _com;

        public SocialCommunityController(ICommunication com, IRepository repos)
        {
            _com = com;

        }

        [HttpGet]
        [Route("GetFollowers")]
        public IHttpActionResult GetFollowers(string userId)
        {
            try
            {
                var followers = _com.GetNodeLinkers<Profile, Profile>(userId, Linkage.Follow);
                return Ok(followers);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }


        [HttpGet]
        [Route("GetBlockedUsers")]
        public IHttpActionResult GetBlockedUsers(string userId)
        {
            try
            {
                var blockedUsers = _com.GetLinkedBy<Profile, Profile>(userId, Linkage.Block);
                return Ok(blockedUsers);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        [HttpGet]
        [Route("GetFollowings")]
        public IHttpActionResult GetFollowings(string userId)
        {

            try
            {
                var followers = _com.GetLinkedBy<Profile, Profile>(userId, Linkage.Follow);

                return Ok(followers);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet]
        [Route("GetPostsByFollowed")]
        public IHttpActionResult GetPostsByFollowed(string followerId)
        {

            try
            {
                var posts = _com.GetNodeLinkedByLinkedByWithLinkersCount<Post, Profile, Profile>(followerId, Linkage.Follow, Linkage.Publish, Linkage.Like);
                foreach (var post in posts)
                {
                    bool isLike =_com.IsLinker<Post, Profile>(post.Id, followerId, Linkage.Like);
                    if (isLike)
                    {
                        post.IsLiked = true;
                    }
                }
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        [HttpGet]
        [Route("GetRecommendedFriends")]
        public IHttpActionResult GetRecommendedFriends(string userId)
        {
            try
            {
                var recommendedFriends = _com.GetLinkers<Profile, Profile>(userId, Linkage.Recommended);
                return Ok(recommendedFriends);
            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpGet]
        [Route("GetRecommendedPosts")]
        public IHttpActionResult GetRecommendedPosts(string userId)
        {
            try
            {
                var recommendedPosts = _com.GetLinkers<Profile, Post>(userId, Linkage.Recommended);
                return Ok(recommendedPosts);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        [Route("GetPosts")]
        public IHttpActionResult GetPosts(string userId)
        {
            try
            {
                var posts = _com.GetLinkedBy<Profile, Post>(userId, Linkage.Publish);
          
                return Ok(posts);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

    }
}
