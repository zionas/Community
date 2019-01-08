using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityNetWork.Dal.Interfaces
{
    public interface IDBConnector:IDisposable
    {
        Task<bool> AddModel(Type type);
        bool RemoveModel(Type type);
        void Add<T>(T model);
        T Get<T>(Primitive hashKey, bool constintence);
    }
}
