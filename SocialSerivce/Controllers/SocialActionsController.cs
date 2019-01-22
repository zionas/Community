
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

using Authentication.BL;

namespace SocialSerivce.Controllers
{
    [RoutePrefix("api/SocialActions")]
    public class SocialActionsController : ApiController
    {
        ICommunication _com;
        IRepository _repos;
        private AmazonS3Uploader amazonS3Uploader;
        private const string BUCKETURL = "https://s3.eu-central-1.amazonaws.com/pictures-bucket32/";

        public SocialActionsController(ICommunication com, IRepository repos)
        {
            _com = com;
            _repos = repos;
            amazonS3Uploader = new AmazonS3Uploader();

        }



        [HttpPost]
        [Route("CreatePost")]
        public IHttpActionResult CreatePost([FromBody]Post post)
        {
            try
            {
                if (post.ImageSourcePath != null)
                {
                    string keyImage = amazonS3Uploader.UploadFile(post.ImageSourcePath);
                    post.ImageSourcePath = BUCKETURL + keyImage;
                }
                _repos.Add(post);
                SocialAction socialAction = new SocialAction()
                {
                    FromId = post.PublisherId,
                    ToId = post.Id,
                    linkage = Linkage.Publish.ToString(),
                    Switcher = true
                };
                _com.LinkProfileToPost(socialAction);
                if (post.TagUserID != null)
                {
                    SocialAction socialAction2 = new SocialAction()
                    {
                        FromId = post.Id,
                        ToId = post.TagUserID,
                        linkage = Linkage.Mention.ToString(),
                        Switcher = true
                    };
                    _com.LinkIf<Profile, Post>(socialAction2);
                }
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
                string res = amazonS3Uploader.UploadFile(file);
                return Ok(res);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("GetComments")]
        public IHttpActionResult GetComments(string postId)
        {
            try
            {
                var comments = _com.GetLinkers<Post, Comment>(postId, Linkage.Comment);
                return Ok(comments);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("AddComment")]
        public IHttpActionResult AddComment([FromBody]Comment comment)
        {
            try
            {
                _repos.Add(comment);
                SocialAction socialAction = new SocialAction()
                {
                    FromId = comment.PublisherId,
                    ToId = comment.Id,
                    linkage = Linkage.Publish.ToString(),
                    Switcher = true
                };
                _com.LinkIf<Comment, Profile>(socialAction);
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
                if (linked)
                {
                    _com.LinkIf<Profile, Profile>(socialAction, true);
                    bool isLinked = _com.IsLinker<Profile, Profile>(toId, fromId, Linkage.Follow);
                    if (isLinked)
                        _com.UnLink<Profile, Profile>(toId, fromId, Linkage.Follow);
                    bool isLinker = _com.IsLinker<Profile, Profile>(fromId, toId, Linkage.Follow);
                    if (isLinker)
                        _com.UnLink<Profile, Profile>(fromId, toId, Linkage.Follow);
                }
                else
                {
                    _com.UnLink<Profile, Profile>(toId, fromId, Linkage.Block);
                }
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
            bool toLink = socialAction.Switcher;
            if (toLink)
                _com.LinkTo(socialAction);
            else
                _com.LinkTo(socialAction, false);
            return Ok(linkage + "s");


        }

        [HttpPost]
        [Route("DoLike")]
        public IHttpActionResult DoLike([FromBody]SocialAction socialAction)
        {
            string fromId = socialAction.FromId;
            string toId = socialAction.ToId;
            Linkage linkage = (Linkage)Enum.Parse(typeof(Linkage), socialAction.linkage);
            bool toLink = socialAction.Switcher;
            if (toLink)
                _com.LinkProfileToPost(socialAction);
            else
            {
                bool a = _com.IsLinker<Post, Profile>(toId, fromId, Linkage.Like);
                _com.UnLink<Post, Profile>(toId, fromId, Linkage.Like);


            }

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
                    bool linker = _com.IsLinker<Profile, Profile>(toId, fromId, linkage);
                    return Ok(linker);
            }


        }


        [HttpPost]
        [Route("GetNotBlocked")]
        public IHttpActionResult GetNotBlocked([FromBody]string blockerId)
        {

            var notLinked = _com.GetNotLinked<Profile, Profile>(blockerId, Linkage.Block);
            return Ok(notLinked);


        }

    }
}
