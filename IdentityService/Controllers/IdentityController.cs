using CommunityNetwork.Common;
using CommunityNetwork.Common.Inerfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IdentityService.Controllers
{
    [RoutePrefix("api/Identity")]
    public class IdentityController : ApiController
    {
        private readonly string errorToken = "The token isn't valid anymore, sign in again";
        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService)
        {
            this._identityService = identityService;
        }

        [HttpPut]
        [Route("Edit")]
        public IHttpActionResult Edit([FromBody]Profile profile)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri("http://localhost:60562/");
                    var response =httpClient.GetAsync($"api/Authentication/CheckValidToken?email={profile.Email}").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var isTokenValid = JsonConvert.DeserializeObject<bool>(response.Content.ReadAsStringAsync().Result);
                        if (!isTokenValid)
                        {
                            return BadRequest(errorToken);
                        }
                    }
                }
                Profile profileEdited = _identityService.Edit(profile);
                return Ok(profile);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
