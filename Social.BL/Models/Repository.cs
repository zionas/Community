using CommunityNetwork.Common.Models;
using CommunityNetWork.Dal;
using CommunityNetWork.Dal.Interfaces;
using Social.BL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.BL.Models
{
    public class Repository:IRepository

    {
        IGraphFactory _graphFactory;
        public Repository(IGraphFactory graphFactory)
        {
            _graphFactory = graphFactory;
        }
        
        public TNode Get<TNode>(string id) where TNode : MNode
        {
            using (IGraph neo4j = (IGraph)_graphFactory.Create())
            {
                return neo4j.Get<TNode>(id);

            }
        }
        public TNode Add<TNode>(TNode node) where TNode : MNode
        {
            using (IGraph graph = (IGraph)_graphFactory.Create())
            {
                return graph.Create(node);

            }
        }
    }
}
