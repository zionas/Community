using CommunityNetwork.Common;
using CommunityNetWork.Common.Enums;
using Social.BL.Interfaces;
using Social.BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NotificationService.Controllers
{
    public class NotificationController : ApiController
    {
        ICommunication _com;
        IRepository _repos;
        public NotificationController(ICommunication com, IRepository repos)
        {
            _com = com;
            _repos = repos;
        }
               

        [HttpPost]
        [Route("SendMessage")]
        public IHttpActionResult SendMessage([FromBody]SocialAction socialAction)
        {
            string fromUser = _repos.Get<Profile>(socialAction.FromId).UserName;
            Linkage linkage = (Linkage)Enum.Parse(typeof(Linkage), socialAction.linkage);
            string subject = linkage.ToString() + "ed";
            switch (linkage) {
                default:
                    return BadRequest();
                
            }//socialAction.FromId;
            string toId = socialAction.ToId;
            
            bool toLink = socialAction.Switcher;
            if (toLink)
                _com.LinkTo(socialAction);
            else
                _com.LinkTo(socialAction, false);
            return Ok(linkage + "s");


        }

        
    }
}
