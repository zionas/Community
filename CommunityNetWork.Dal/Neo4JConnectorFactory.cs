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
        public IDBConnector Create(bool useLocal)
        {
            return new Neo4jConnector(useLocal);
        }
    }
}
