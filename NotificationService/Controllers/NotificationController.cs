
using CommunityNetwork.Common.Models;
using NotificationService.Models;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace NotificationService.Controllers
{
    [RoutePrefix("api/Notification")]
    public class NotificationController : ApiController
    {

        private NotificationSender _notificationSender;

        public NotificationController()
        {
             _notificationSender = new NotificationSender();
        }
        [HttpGet]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(string profileId)
        {
            try
            {
                NotificationSender notificationSender = new NotificationSender();

                await notificationSender.Register(profileId,profileId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("SendMessage")]
        public async Task<IHttpActionResult> SendMessage([FromBody]SocialAction socialAction)
        {
            try
            {
                NotificationSender notificationSender = new NotificationSender();
                
                await notificationSender.Send(socialAction);
                return Ok();
                    }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        
    }
}
