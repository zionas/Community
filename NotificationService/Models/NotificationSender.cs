using CommunityNetwork.Common.Models;
using Notification;
using NotificationService.XMPP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace NotificationService.Models
{
    public class NotificationSender
    {
        readonly XMPPClient xmpp = new XMPPClient();
        const string _userName = "admin"; //"admin@desktop-i6t30no";
        const string _password = "admin";
        //public static async Task<NotificationSender> GetInstance()
        //{
        //    NotificationSender sender = new NotificationSender();
        //    await sender.xmpp.Connect(_userName, _password);
        //    return sender;
        //}

        public NotificationSender() { }

        public async Task Register(string userName, string password)
        {
            await xmpp.Register(userName, password);
        }
        public async Task Send(SocialAction socialAction)
        {
            NotifivationMessage notification = new NotifivationMessage(socialAction);
           
            string userName = notification.TargetName;
            string msg = notification.ToJson();
            await xmpp.Connect(_userName, _password);
            await xmpp.SendPrivateMessage(msg, userName);
                      
        }
    }
}