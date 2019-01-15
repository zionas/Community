using CommunityNetwork.Common;
using CommunityNetwork.Common.Models;
using Social.BL.Interfaces;
using Social.BL.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SocialSerivce.Controllers
{
    [RoutePrefix("api/Publish")]
    public class PublishController : ApiController
    {
        private readonly IPublisher _publisher;
        public PublishController(IPublisher publisher)
        {
            _publisher = publisher;
        }


        [HttpPost]
        [Route("PublishPost")]
        public IHttpActionResult PublishPost([FromBody]PublishAction publishAction)
        {
            try
            {
                Post post = (Post)publishAction.Publish;
                string authorId = publishAction.AuthorId;
                Post p = _publisher.Publish(authorId, post);
                return Ok(p);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("Comment")]
        public IHttpActionResult Comment([FromBody]PublishAction publishAction)
        {
            try
            {
                Comment comment = (Comment)publishAction.Publish;
                string commentedId = publishAction.CommentedId;
                string authorId = publishAction.AuthorId;


                if (comment == default(Comment)
                   || commentedId == default(string)
                   || authorId == default(string))
                    return BadRequest();

                Comment c = _publisher.Comment<Post>(authorId, comment, commentedId);
                if (c.Equals(comment))
                    return Ok(c);
                else
                    return ResponseMessage(new HttpResponseMessage(HttpStatusCode.ExpectationFailed));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


    }
}
