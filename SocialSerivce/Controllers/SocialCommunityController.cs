using CommunityNetwork.Common;
using CommunityNetwork.Common.Models;
using CommunityNetWork.Common.Enums;
using Social.BL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
                var followers = _com.GetLinkers<Profile, Profile>(userId, Linkage.Follow);
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
                var blockedUsers = _com.GetLinkers<Profile, Profile>(userId, Linkage.Block);
                return Ok(blockedUsers);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        [HttpPost]
        [Route("GetFollowed")]
        public IHttpActionResult GetFollowed([FromBody]string blockerId)
        {

            var followers = _com.GetLinkedBy<Profile, Profile>(blockerId, Linkage.Block);

            return Ok(followers);

        }

        [HttpPost]
        [Route("GetPostsByFollowed")]
        public IHttpActionResult GetPostsByFollowed([FromBody]string blockerId)
        {

            var followers = _com.GetLinkedByLinkedBy<Profile, Profile, Post>(blockerId, Linkage.Follow, Linkage.Publish);

            return Ok(followers);


        }

        [HttpGet]
        [Route("GetPosts")]
        public IHttpActionResult GetPosts(string userId)
        {
            try
            {
                var followings = _com.GetLinkedBy<Profile, Profile>(userId, Linkage.Follow);
                List<Post> posts = new List<Post>();
                foreach (var f in followings)
                {
                    //posts = _com.GetLinkers<Post,Profile>(f.Id, Linkage.Publish);
                }
                return Ok(posts);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

    }
}
