using CommunityNetwork.Common.Models;
using CommunityNetWork.Common.Enums;
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
            using (IGraph praph = (IGraph)_graphFactory.Create())
            {
                return praph.Get<TNode>(id);

            }
        }

        public string GetTypeName(string id) 
        {
            using (IGraph graph = (IGraph)_graphFactory.Create())
            {
                return graph.GetTypeName(id);

            }
        }
        public List<TNode> Get<TNode>(string propertyName,object value) 
            where TNode : MNode
        {
            using (IGraph graph = (IGraph)_graphFactory.Create())
            {
                return graph.Get<TNode>(propertyName,value);

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
