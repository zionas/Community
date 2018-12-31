using CommunityNetwork.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityNetwork.Common
{
    public class Profile
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string WorkPlace { get; set; }
        public DateTime CreateTime { get; set; }

        public List<string> GetBlocked()
        {
            // neo4j 
            return null;
        }

        public void DoBlock(string profileName)
        {

        }

        public void CreateNewPost(Post post)
        {

        }
    }
}
