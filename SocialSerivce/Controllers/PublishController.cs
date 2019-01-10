using CommunityNetwork.Common;
using CommunityNetwork.Common.Models;
using Social.BL.Interfaces;
using Social.BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SocialSerivce.Controllers
{
    public class PublishController : ApiController
    {
        private readonly IPublisher _publisher;
        public PublishController(IPublisher publisher)
        {
            _publisher = publisher;
        }
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Publish/5
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        [Route("PublishPost")]
        public IHttpActionResult PublishPost([FromBody]PublishAction publishAction)
        {
            Post post = (Post)publishAction.Publish;
            string authorId = publishAction.AuthorId;
            Post p = _publisher.Publish(authorId, post);
            if (p.Equals(post))
                return Ok(p);
            else
                return Content(HttpStatusCode.InternalServerError, false);
        }
        [HttpPost]
        [Route("Comment")]
        public IHttpActionResult Comment([FromBody]PublishAction publishAction)
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

        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Publish/5
        public void Delete(int id)
        {
        }
    }
}
