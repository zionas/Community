using CommunityNetwork.Common;
using CommunityNetwork.Common.Models;
using CommunityNetWork.Common.Enums;
using Social.BL.Interfaces;
using Social.BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification
{
    public  class NotifivationMessage
    {
        SocialAction _socialAction { get;  }
        IRepository _repository;
        IPublisher _publisher;
        public NotifivationMessage(SocialAction socialAction, Repository repository,Publisher publisher)
        {
            _socialAction = socialAction;
            _repository = repository;
            _publisher = publisher;

        }
        public override string ToString()
        {
            string id = _socialAction.FromId;
            string nodeName = _repository.GetTypeName(id);
                
            string linkage = _socialAction.linkage;
            string actor="";
            string acted=linkage+"ed";
            string subjected="";
            switch (Enum.Parse(typeof(Linkage), linkage))
            {

                default:
                    break;
                case Linkage.Comment:
                    subjected = " on your " + nodeName;
                    actor = _publisher.GetPublisher<Comment>(id).UserName;
                    break;
                case Linkage.Follow:
                    subjected = " you ";
                    actor = _repository.Get<Profile>(id).UserName;
                    break;
                case Linkage.Like:
                    subjected = " your " + nodeName;
                    actor = _repository.Get<Profile>(id).UserName;
                    break;
            }
            if (subjected != "" && actor != "")
                return actor + acted + subjected;
            else
                return default(string);


            

            

            
        }
    }
}
