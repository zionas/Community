using CommunityNetWork.Dal.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityNetWork.Dal
{
    public class AwsDynamoDBFactory:IDynamoDBFactory
    {
        public IDynamoDB Create(bool useLocal)
        {
            return new DynamoDBConnector(useLocal);
        }
    }
}
