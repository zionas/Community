using CommunityNetwork.Common;
using CommunityNetWork.Dal.Interfaces;
using Social.BL.Models;
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
        // GET: api/Persistence
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Persistence/5
        public string Get(int id)
        {
            return "value";
        }
        [HttpPost]
        [Route("CreateProfile")]
         public IHttpActionResult CreateProfile([FromBody]Profile profile)
        {
            Profile p=repos.Add(profile);
            if (p.Equals(profile))
                return Ok(p);//("Created" + p.Id);
            else
                return Content(HttpStatusCode.InternalServerError, false);
        }
        // POST: api/Persistence
        public IHttpActionResult Post([FromBody]Profile profile)
        {
            Profile p = repos.Add(profile);
            if (p.Equals(profile))
                return Ok("created" + p.Id);
            else
                return Content(HttpStatusCode.InternalServerError, false);
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
