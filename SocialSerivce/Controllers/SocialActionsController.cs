
using CommunityNetWork.Common.Enums;
using Social.BL.Interfaces;
using SocialSerivce.Models;
using System;
using System.Collections.Generic;
using System.Web.Http;
using CommunityNetwork.Common.Inerfaces;
using CommunityNetwork.Common;

namespace SocialSerivce.Controllers
{
    [RoutePrefix("api/SocialActions")]
   
    public class SocialActionsController : TokenedApiController
    {
        ICommunication _com;
        IRepository _repos;
        public SocialActionsController(ICommunication com,IRepository repos)//IGraphFactory graphFactory)
        {
            _com = com;
            _repos = repos;
        }
         
        // GET: api/SocialActions
        public IHttpActionResult Get()
        {

            if (!IsAuthorized())
                
                return BadRequest("not authorized");
            return Ok(new string[] { "value1","value2" });
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
        [HttpPost]
        [Route("IsLinked")]
        public IHttpActionResult IsLinked([FromBody]SocialAction socialAction)
        {
            Guid fromId = socialAction.FromId;
            Guid toId = socialAction.ToId;
            Linkage linkage = (Linkage)Enum.Parse(typeof(Linkage), socialAction.linkage);
            bool linked=_com.IsLinked<INode,INode>(fromId, toId,linkage);
            return Ok(linked);


        }
        [HttpPost]
        [Route("GetNotBlocked")]
        public IHttpActionResult GetNotBlocked([FromBody]Guid blockerId)
        {
           
            var notLinked= _com.GetNotLinked<Profile, Profile>(blockerId,Linkage.Block);
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
