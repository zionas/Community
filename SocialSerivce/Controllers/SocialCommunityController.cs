using CommunityNetwork.Common;
using CommunityNetwork.Common.Models;
using CommunityNetWork.Common.Enums;
using Social.BL.Interfaces;
using System;
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
        public IHttpActionResult GetFollowed([FromBody]string followerId)
        {

            try
            {
                var followers = _com.GetLinkedBy<Profile, Profile>(followerId, Linkage.Block);

                return Ok(followers);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost]
        [Route("GetPostsByFollowed")]
        public IHttpActionResult GetPostsByFollowed([FromBody]string followerId)
        {

            try
            {
                var followers = _com.GetNodeLinkedByLinkedByWithLinkersCount<Post, Profile, Profile>(followerId, Linkage.Follow, Linkage.Publish, Linkage.Like);
                return Ok(followers);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

    }
}
