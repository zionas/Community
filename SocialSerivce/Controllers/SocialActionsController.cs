
using CommunityNetWork.Common.Enums;
using Social.BL.Interfaces;
using Social.BL.Models;
using System;
using System.Web.Http;
using CommunityNetwork.Common;
using CommunityNetwork.Common.Models;
using System.IO;
using System.Web;
using Amazon.S3;
using Amazon;
using Amazon.S3.Transfer;
using Social.BL;

namespace SocialSerivce.Controllers
{
    [RoutePrefix("api/SocialActions")]

    public class SocialActionsController : TokenedApiController
    {
        ICommunication _com;
        IRepository _repos;

        public SocialActionsController(ICommunication com, IRepository repos)
        {
            _com = com;
            _repos = repos;
        }

        // GET: api/SocialActions
        public IHttpActionResult Get()
        {

            if (!IsAuthorized())

                return BadRequest("not authorized");
            return Ok(new string[] { "value1", "value2" });
        }

        // GET: api/SocialActions/5
        public string Get(int id)
        {
            return "value";
        }





        [HttpPost]
        [Route("CreatePost")]
        public IHttpActionResult CreatePost([FromBody]Post post)
        {
            try
            {
                _repos.Add(post);
                SocialAction socialAction = new SocialAction()
                {
                    FromId = post.PublisherId,
                    ToId = post.Id,
                    linkage = Linkage.Publish.ToString(),
                    Switcher = true
                };
                _com.LinkProfileToPost(socialAction);
                return Ok();


            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
        [HttpPost]
        [Route("Upload")]
        public IHttpActionResult Upload([FromBody]string file)
        {
            try
            {
                AmazonS3Uploader amazonS3Uploader = new AmazonS3Uploader();
                string res =amazonS3Uploader.UploadFile(file);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("BlockUser")]
        public IHttpActionResult BlockUser([FromBody]SocialAction socialAction)
        {
            try
            {
                string fromId = socialAction.FromId;
                string toId = socialAction.ToId;
                Linkage linkage = (Linkage)Enum.Parse(typeof(Linkage), socialAction.linkage);
                bool linked = socialAction.Switcher;
                bool flag = _com.IsLinked<Profile, Profile>(fromId, toId, linkage);
                if(flag)
                    _com.UnLink<Profile, Profile>(fromId, toId, Linkage.Follow);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpPost]
        [Route("SWLinkProfiles")]
        public IHttpActionResult SWLinkProfiles([FromBody]SocialAction socialAction)
        {
            string fromId = socialAction.FromId;
            string toId = socialAction.ToId;
            Linkage linkage = (Linkage)Enum.Parse(typeof(Linkage), socialAction.linkage);
            bool linked = socialAction.Switcher;
            if (linked)
                _com.LinkProfiles(socialAction);
            else
                _com.LinkProfiles(socialAction, false);
            return Ok(linkage + "s");


        }



        [HttpPost]
        [Route("IsSocialLinked")]
        public IHttpActionResult IsSocialLinked([FromBody]SocialAction socialAction)
        {
            string fromId = socialAction.FromId;
            string toId = socialAction.ToId;
            Linkage linkage = (Linkage)Enum.Parse(typeof(Linkage), socialAction.linkage);

            switch (linkage)
            {
                default:
                    return Ok(false);
                case Linkage.Block:
                case Linkage.Follow:
                    bool linked = _com.IsLinked<Profile, Profile>(fromId, toId, linkage);
                    return Ok(linked);
            }


        }





        [HttpPost]
        [Route("GetNotBlocked")]
        public IHttpActionResult GetNotBlocked([FromBody]string blockerId)
        {

            var notLinked = _com.GetNotLinked<Profile, Profile>(blockerId, Linkage.Block);
            return Ok(notLinked);


        }
        // PUT: api/SocialActions/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/SocialActions/5
        public void Delete(int id)
        {
        }
    }
}
