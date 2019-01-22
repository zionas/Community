using CommunityNetwork.Common;
using CommunityNetwork.Common.Models;
using CommunityNetWork.Dal.Interfaces;
using Newtonsoft.Json;
using Social.BL.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
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
            try
            {
                Profile newProfile = repos.Add(profile);

                return Ok(newProfile);

            }
            catch (Exception ex)
            {
                return BadRequest("Error occured," + ex.Message);
            }


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

        [HttpGet]
        [Route("GetProfileByUserId")]
        public IHttpActionResult GetProfileByUserId(string userId)
        {
            try
            {
                var profiles = repos.Get<Profile>("Id", userId);
                return Ok(profiles);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }




    }
}
