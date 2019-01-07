
using CommunityNetWork.Common.Enums;
using CommunityNetWork.Dal.Interfaces;
using Social.BL.Models;
using Social.BL.Interfaces;
using SocialSerivce.Models;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace SocialSerivce.Controllers
{
    [RoutePrefix("api/SocialActions")]
   
    public class SocialActionsController : ApiController
    {
        ICommunication _com;
        IRepository _repos;
        public SocialActionsController(ICommunication com,IRepository repos)//IGraphFactory graphFactory)
        {
            _com = com;
            _repos = repos;
        }
         
        // GET: api/SocialActions
        public IEnumerable<string> Get()
        {
            return new string[] { "value1","value2" };
        }

        // GET: api/SocialActions/5
        public string Get(int id)
        {
            return "value";
        }
        [HttpPost]
        [Route("Follow")]
        public IHttpActionResult Follow([FromBody]SocialAction socialAction)
        {
            Guid fromId = socialAction.FromId;
            Guid toId = socialAction.ToId;
            Linkage linkage = (Linkage) Enum.Parse(typeof(Linkage),socialAction.linkage);
            _com.Follow(fromId,toId);
            return Ok("Follows");
                

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
