

using System;

using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;

namespace CommunityNetWork.Dal.Interfaces
{
    public interface IDynamoDB:IDBConnector
    {

        Task<bool> AddModel(Type type);
        bool RemoveModel(Type type);
        void Add<T>(T model);
        T Get<T>(Primitive hashKey, bool constintence);
    }
}



