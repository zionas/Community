using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityNetwork.Common.Inerfaces
{
    public interface INode
    {
        Guid Id { get; set; }
        void ResetId();
    }
    
}
