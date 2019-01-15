using CommunityNetwork.Common.Models;
using CommunityNetWork.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.BL.Interfaces
{
    public interface IRepository
    {
        string GetTypeName(string id);
        TNode Get<TNode>(string id) where TNode : MNode;
        List<TNode> Get<TNode>(string propertyName, object value) where TNode : MNode;
        TNode Add<TNode>(TNode node) where TNode : MNode;

    }
}
