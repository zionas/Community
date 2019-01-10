using CommunityNetwork.Common;
using CommunityNetWork.Dal.Interfaces;
using Newtonsoft.Json;
using Social.BL.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace SocialSerivce.Controllers
{
    [RoutePrefix("api/Persistence")]
    public class PersistenceController : ApiController
    {
        Repository repos;
        
        public PersistenceController(IGraphFactory graphFactory)
        {
            
            repos = new Repository(graphFactory);
        }
        

        [HttpPost]
        [Route("CreateProfile")]
         public IHttpActionResult CreateProfile([FromBody]Profile profile)
        {
            Profile p=repos.Add(profile);
            if (p.Equals(profile))
                return Ok(p);
            else
                return Content(HttpStatusCode.InternalServerError, false);
        }

        [HttpGet]
        [Route("GetProfileByUserName")]
        public IHttpActionResult GetProfileByUserName(string userName)
        {
            try
            {
                List<Profile> profiles = repos.Get<Profile>("UserName", userName);
                return Ok(profiles);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        // PUT: api/Persistence/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Persistence/5
        public void Delete(int id)
        {
        }
    }
}
