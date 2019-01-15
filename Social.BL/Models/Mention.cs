using CommunityNetwork.Common;
using CommunityNetwork.Common.Models;
using CommunityNetWork.Common.Enums;
using CommunityNetWork.Dal.Interfaces;
using Social.BL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.BL.Models
{
    //header format [profileId1 start end, profileId2 start end]
    public  class Mention
    {
        private ICommunication _comm;
        private const string end = "]";
        private const char profilesDelimiter = ',';
        public Mention()
        {
            _comm = (ICommunication)Config.GetInstance<ICommunication>();
        }
        static List<string> GetMentionedIds(string text)
        {
            int endHeader = text.IndexOf(end);
            string header = text.Substring(0,endHeader);
                        
            return header
                   .Split(profilesDelimiter)
                   .Select(s=>s.Split()[0]).ToList();
        }

         int LinkMentions(string postId,List<string> profileIds)
        {
           
            profileIds.ForEach(id => _comm.Link<Profile, Post>(id, postId, Linkage.Mention));
            return profileIds.Count();
        }

        int UnLinkMentions(string postId, List<string> profileIds)
        {
            
            profileIds.ForEach(id => _comm.UnLink<Profile, Post>(id, postId, Linkage.Mention));
            return profileIds.Count();
        }

         List<string> GetMentioned(string postId,List<string> profileIds)
        {
         return profileIds.Where(
                    id => _comm.IsLinker<Profile, Post>(
                       id, postId, Linkage.Mention)).ToList();
            
        }
         void EditMentions(string postId, List<string> profileIds)
        {

            List<string> mentionedProfileIds = GetMentioned(postId, profileIds);
            LinkMentions(postId,
                         profileIds.
                         Except(mentionedProfileIds)
                         .ToList());

            UnLinkMentions(postId,
                mentionedProfileIds
                .Except(profileIds)
                .ToList());
        }

        
    }
}
