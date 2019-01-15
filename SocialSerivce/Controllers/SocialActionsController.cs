
using CommunityNetWork.Common.Enums;
using Social.BL.Interfaces;
using Social.BL.Models;
using System;
using System.Web.Http;
using CommunityNetwork.Common;
using Authentication.BL;

namespace SocialSerivce.Controllers
{
    [RoutePrefix("api/SocialActions")]
    [AuthorizeValidator ]
    [Authorize]
    public class SocialActionsController : ApiController
    {
        ICommunication _com;
        IRepository _repos;
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
            if (toLink)
                _com.LinkProfiles(socialAction);
            else
                _com.LinkProfiles(socialAction, false);
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
