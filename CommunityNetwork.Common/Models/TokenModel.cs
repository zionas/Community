using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityNetwork.Common.Models
{
    public class TokenModel
    {
        
        [DynamoDBHashKey]
        public string Token { get; set; }

        public DateTime TokenCreateTime { get; set; }
        public string Email { get; set; }

        public bool CheckTokenValid(DateTime tokenCreateTime)
        {
            throw new NotImplementedException();
        }
    }
}
