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
        [Route("GetTypeName")]
        public IHttpActionResult GetTypeName(string id)
        {
            try
            {
                string name = repos.GetTypeName(id);
                if (string.IsNullOrEmpty(name))
                    return BadRequest();

                return Ok(name);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
        [Route("GetProfileUserName")]
        public IHttpActionResult GetProfileUserName(string id)
        {
            try
            {
                Profile profile = repos.Get<Profile>(id);
                return Ok(profile.UserName);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


    }
}
