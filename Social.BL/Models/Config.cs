using CommunityNetWork.Dal.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityNetWork.Dal;
using Social.BL.Interfaces;

namespace Social.BL.Models
{
    public  class Config
    {
        private static IGraphFactory _graphFactory;
        private static ICommunication _communication;

        static private Config _config = new Config();
        public IGraphFactory GraphFactory { get; set; }
        private Config()
        {
            _graphFactory = _graphFactory ?? new Neo4JConnectorFactory();
            _communication = new Communication(_graphFactory);
        }
        public static object GetInstance<TConstructorParam>()
        {
            return _config.GetType()
                .GetProperty(typeof(TConstructorParam).Name)
                .GetValue(_config);
        }
        

        
    }
}
