using CommunityNetwork.Common;
using CommunityNetwork.Common.Models;
using CommunityNetWork.Common.Enums;
using Social.BL.Models;
using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace Notification
{
    public  class NotifivationMessage
    {
        
        Linkage _linkage;
        
        const string SocialServiceUri = "http://localhost:52225/";
        public string TargetName { get;  }
        string actorName;
        
        string headerOfIds;
        string _actorId;
        string _objectId;
        


        public NotifivationMessage(SocialAction socialAction)
        {
            string toId = socialAction.ToId;
            string fromId = socialAction.FromId;
            
            _linkage = (Linkage)Enum.Parse(typeof(Linkage), socialAction.linkage);
            TargetName = GetSendToId(toId);
            _actorId = _linkage==Linkage.Follow?fromId:GetActorModel(fromId).Id;
            _objectId = GetObjectId(toId);
                       
        }
        public NotifivationMessage(SocialAction socialAction,int i)
        {
            string toId = socialAction.ToId;
            string fromId = socialAction.FromId;
            actorName = GetActorModel(fromId).UserName;
            TargetName = GetModelName(toId);
            _linkage = (Linkage)Enum.Parse(typeof(Linkage), socialAction.linkage);
            _actorId = _linkage == Linkage.Follow ? fromId : GetActorModel(fromId).Id;
            string sendToId = GetSendToId(toId);
            _objectId = GetObjectId(toId);
            headerOfIds = _actorId + "," + sendToId;
            headerOfIds = _objectId != null ? headerOfIds + _objectId + "," + "]" : "]";
        }

        string GetSendToId(string toId)
        {
            if (_linkage == Linkage.Follow)
                return toId;
            else
                return GetTargetId(toId);
        }


        public override string ToString()
        {
            

            if (actorName == default(string))
            {
                return default(string);
            }
            string acted = _linkage + "ed";
           
            switch (_linkage)
            {
                default:
                    return default(string);
                case Linkage.Comment:
                   return  headerOfIds+ actorName+ acted +" on your " + TargetName;
                case Linkage.Follow:
                    return headerOfIds + actorName + acted + " you ";
                case Linkage.Like:
                    return headerOfIds + actorName + acted + " your " + TargetName;
                case Linkage.Mention:
                    return headerOfIds + actorName + acted + " you in the " +TargetName;
             }

        }
        public  string ToJson()
        {


            CommunityNetwork.Common.Models.Notification notification = new CommunityNetwork.Common.Models.Notification
            {
                ActorId = _actorId,
                linkage = _linkage.ToString(),
                ObjectId = _objectId
            };
            return JsonConvert.SerializeObject(notification);

        }

        private Profile GetActorModel(string fromId)
        {
             if (_linkage==Linkage.Comment)
                    return GetActorModel(fromId, "Publish/GetCommentAuthorName");
                else
                    return GetActorModel(fromId,
                                                "Persistence/GetProfileUserName");
        }


        private Profile GetActorModel(string id,string apiPath)
        {
            Profile actor = default(Profile);
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(SocialServiceUri);
                var response =   httpClient.GetAsync($"api/"+apiPath+"?id={id}").Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    actor = JsonConvert.DeserializeObject<Profile>(content);
                }
                
            }
            return actor;
        }

        private string GetModelName(string id)
        {
            string name = default(string);
            string apiPath = "Persistence/GetTypeName";
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(SocialServiceUri);
                var response = httpClient.GetAsync($"api/" + apiPath + "?id={id}").Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    name = JsonConvert.DeserializeObject<string>(content);
                }
                
            }
            return name;
        }

        private string GetTargetId(string toId)
        {
             switch (_linkage)
            {
                default:
                    return default(string);
                case Linkage.Comment:
                case Linkage .Like:
                    if (GetModelName(toId)=="Post")
                        return GetActorModel(toId,  "Publish/GetPostAuthorName").Id; 
                    else
                        return GetActorModel(toId,  "Publish/GetCommentAuthorName").Id;
                case Linkage.Follow:
                case Linkage.Mention:
                    return toId;

            }

           
        }

        private string GetObjectId(string toId)
        {
            switch (_linkage)
            {
                default:
                    return default(string);
                case Linkage.Comment:
                case Linkage.Like:
                case Linkage.Mention:
                    if (GetModelName(toId) == "Post")
                        return GetActorModel(toId, "Publish/GetPostAuthorName").Id;
                    else
                        return GetActorModel(toId, "Publish/GetCommentAuthorName").Id;

            }
        }

    }
}
