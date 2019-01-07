using CommunityNetWork.Dal.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityNetWork.Dal
{
    public class Neo4JConnectorFactory : IGraphFactory
    {
        public IGraph Create()
        {
            return new Neo4jConnector();
        }
    }
}
