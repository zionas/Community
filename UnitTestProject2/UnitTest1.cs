using System;
using System.Threading.Tasks;
using CommunityNetwork.Common.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NotificationService.Models;
using Social.BL.Models;

namespace UnitTestProject2
{
   
    [TestClass]
    public class UnitTest1
    {
        SocialAction _socialAction = new SocialAction
        {
            FromId = "2222",
         ToId= "1111",
         linkage="Follow",
         Switcher=true

        };
        [TestMethod]
        public async Task TestConnect()
        {
            NotificationSender sender =  new NotificationSender();
            await sender.Register("88", "88");
        }
        [TestMethod]
        public async Task TestSend()
        {
            NotificationSender sender = new NotificationSender();
            
            await sender.Send(_socialAction);
        }
    }
}
