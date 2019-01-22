
using CommunityNetWork.Common.Enums;
using Social.BL.Interfaces;
using Social.BL.Models;
using System;
using System.Web.Http;
using CommunityNetwork.Common;
using Authentication.BL;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using CommunityNetwork.Common.Models;

namespace SocialSerivce.Controllers
{
    [RoutePrefix("api/SocialActions")]
    [AuthorizeValidator ]
    [Authorize]
    public class SocialActionsController : ApiController
    {
        ICommunication _com;
        IRepository _repos;
        const string NotificationServiceUri = "http://localhost:54169/";
        const string SendMessageApi= NotificationServiceUri+"/api/Notification";

        bool SendMessage(SocialAction socialAction)
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    httpClient.BaseAddress = new Uri(NotificationServiceUri);
                    var content = new StringContent(JsonConvert.SerializeObject(socialAction), Encoding.UTF8, "application/json");
                    var response = httpClient.PostAsync(SendMessageApi, content).Result;
                    if (response.IsSuccessStatusCode)
                        return true;
                    else
                        return false;
                }
                catch (Exception ex)
                {
                    return false;//skip this exception
                }
            }       
                
        }
        public SocialActionsController(ICommunication com,IRepository repos)
        {
            _com = com;
            _repos = repos;
        }
         
        
        
        [HttpPost]
        [Route("SWLinkProfiles")]
        public IHttpActionResult SWLinkProfiles([FromBody]SocialAction socialAction)
        {
            string fromId = socialAction.FromId;
            string toId = socialAction.ToId;
            Linkage linkage = (Linkage)Enum.Parse(typeof(Linkage), socialAction.linkage);
            bool toLink = socialAction.Switcher;
            try
            {
                if (toLink)
                {
                    _com.LinkProfiles(socialAction);
                    SendMessage(socialAction);

                }

                else
                    _com.LinkProfiles(socialAction, false);
            }
            catch(Exception ex)
            {
                return BadRequest();
            }
            
            return Ok(linkage+"s");


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
           
            var notLinked= _com.GetNotLinked<Profile, Profile>(blockerId,Linkage.Block);
            return Ok(notLinked);


        }
       
    }
}
