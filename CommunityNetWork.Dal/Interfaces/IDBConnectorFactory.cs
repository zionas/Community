using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityNetWork.Dal.Interfaces
{
    public interface IDBConnectorFactory
        
    {
        IDBConnector Create(bool useLocal = true);
    }
}
