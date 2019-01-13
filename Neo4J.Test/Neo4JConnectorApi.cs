using System;
using System.Collections.Generic;
using CommunityNetwork.Common.Models;
using CommunityNetWork.Common.Enums;
using CommunityNetWork.Dal;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Neo4J.Test
{
    
     class NodeA : MNode
    {
        public int IntProperty { get; set; }
        public string StringProperty { get; set; }
        public override bool Equals(object obj)
        {
            if (!(obj is NodeA))
                return false;
            NodeA n = (NodeA)obj;
            return Id.Equals(n.Id);
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
    [TestClass]
    public class Neo4JConnectorApi
    {
        NodeA nA1,nA2,nA3,nA4;
        public void Init()
        {
            nA1= new NodeA
            {
                Id = Guid.NewGuid().ToString(),
                IntProperty = 1,
                StringProperty = "1"


            };
            nA2 = new NodeA
            {
                Id = Guid.NewGuid().ToString(),
                IntProperty = 2,
                StringProperty = "2"


            };
            nA3 = new NodeA
            {
                Id = Guid.NewGuid().ToString(),
                IntProperty = 3,
                StringProperty = "3"


            };
            nA4 = new NodeA
            {
                Id = Guid.NewGuid().ToString(),
                IntProperty = 4,
                StringProperty = "3"


            };
        }
        
    [TestMethod]
        public void TestNodeWasCreated()
        {
            NodeA  node2;
            Init();
            using (Neo4jConnector neo4j = new Neo4jConnector())
            {
                node2 = neo4j.Create(nA1);
            }
                bool result = nA1.Equals(node2);
                Assert.AreEqual(result, true);
       }

        [TestMethod]
        public void TestGetNodes()
        {
            NodeA node;
            List<NodeA> nodes;
            Init();
            using (Neo4jConnector neo4j = new Neo4jConnector())
            {
                node = neo4j.Create<NodeA>(nA1);
                nodes = neo4j.Get<NodeA>();
            }
            bool result = nodes.Contains(nA1);
            Assert.AreEqual(result, true);
        }


        [TestMethod]
        public void TestDeleteNodes()
        {
            NodeA node;
            List<NodeA> nodes,empty;
            Init();
            using (Neo4jConnector neo4j = new Neo4jConnector())
            {
                node = neo4j.Create(nA1);
                nodes = neo4j.Get<NodeA>();
                neo4j.Delete<NodeA>();
                empty= neo4j.Get<NodeA>();
            }
            bool result = nodes.Contains(nA1)&&empty.Count==0;
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void TestGetNodeByItsId()
        {
            NodeA  node2;
            Init();
            using (Neo4jConnector neo4j = new Neo4jConnector())
            {
                neo4j.Create(nA1);
                node2 = neo4j.Get<NodeA>(nA1.Id);
            }
            bool result = nA1.Equals(node2);
            Assert.AreEqual(result, true);
        }


        [TestMethod]
        public void TestGetNodeByItsProperty()
        {
            List<NodeA> nodes;
            Init();
            using (Neo4jConnector neo4j = new Neo4jConnector())
            {
                neo4j.Delete<NodeA>();
                neo4j.Create(nA1);
                neo4j.Create(nA2);
                nodes = neo4j.Get<NodeA>("StringProperty", nA2.StringProperty);
            }
            bool result = nodes.Count==1&&nA2.Equals(nodes[0]);
            Assert.AreEqual(result, true);
        }


        [TestMethod]
        public void TestLinkNode()
        {
            bool result;
            Init();
            using (Neo4jConnector neo4j = new Neo4jConnector())
            {
                neo4j.Create(nA1);
                neo4j.Create(nA2);
                neo4j.Link<NodeA, NodeA>(nA1.Id, nA2.Id, Linkage.Follow);
                result = neo4j.IsLinker<NodeA, NodeA>(nA1.Id, nA2.Id, Linkage.Follow);
                
            }
            
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void TestGetNodeLinks()
        {
            List<NodeA> list;
            Init();
            using (Neo4jConnector neo4j = new Neo4jConnector())
            {
                neo4j.Create(nA1);
                neo4j.Create(nA2);
                neo4j.Create(nA3);
                neo4j.Link<NodeA,NodeA>(nA1.Id, nA2.Id, Linkage.Follow);
                neo4j.Link<NodeA, NodeA>(nA1.Id, nA3.Id, Linkage.Follow);
                list = neo4j.GetNodeLinkers<NodeA,NodeA>(nA1.Id, Linkage.Follow);
            }
            bool result = list.Contains(nA2)&& list.Contains(nA3);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void TestGetNodeLinkers()
        {
            List<NodeA> list;
            Init();
            using (Neo4jConnector neo4j = new Neo4jConnector())
            {
                neo4j.Create(nA1);
                neo4j.Create(nA2);
                neo4j.Create(nA3);
                neo4j.Link<NodeA, NodeA>(nA1.Id, nA2.Id, Linkage.Follow);
                neo4j.Link<NodeA, NodeA>(nA1.Id, nA3.Id, Linkage.Follow);
                list = neo4j.GetNodeLinkers<NodeA, NodeA>(nA1.Id, Linkage.Follow);
            }
            bool result = list.Contains(nA2) && list.Contains(nA3);
            Assert.AreEqual(result, true);
        }
        [TestMethod]
        public void TestGetNodeNotLinks()
        {
            List<NodeA> list;
            Init();
            using (Neo4jConnector neo4j = new Neo4jConnector())
            {
                neo4j.Delete<NodeA>();
                neo4j.Create(nA1);
                neo4j.Create(nA2);
                neo4j.Create(nA3);
                neo4j.Link<NodeA, NodeA>(nA1.Id, nA2.Id, Linkage.Follow);
                
                list = neo4j.GetNodeNotLinks<NodeA, NodeA>(nA1.Id, Linkage.Follow);
            }

            bool result = !list.Contains(nA2) && list.Contains(nA3);
            Assert.AreEqual(result, true);
        }

        
        [TestMethod]
        public void TestGetNodeLinkedByLinkedBy()
        {
            List<NodeA> list;
            Init();
            using (Neo4jConnector neo4j = new Neo4jConnector())
            {
                neo4j.Delete<NodeA>();
                neo4j.Create(nA1);
                neo4j.Create(nA2);
                neo4j.Create(nA3);
                neo4j.Link<NodeA, NodeA>(nA2.Id, nA1.Id, Linkage.Follow);
                neo4j.Link<NodeA, NodeA>(nA3.Id, nA2.Id, Linkage.Follow);
                
                list = neo4j.GetNodeLinkedByLinkedBy<NodeA, NodeA,NodeA>(nA1.Id, Linkage.Follow,Linkage.Follow);
                
            }

            bool result =  list.Contains(nA3);
            Assert.AreEqual(result, true);
        }


        [TestMethod]
        public void TestGetNodesLinks()
        {
            Dictionary<NodeA,List<NodeA>> dict;
            Init();
            using (Neo4jConnector neo4j = new Neo4jConnector())
            {
                neo4j.Create(nA1);
                neo4j.Create(nA2);
                neo4j.Create(nA3);
                neo4j.Link<NodeA, NodeA>(nA1.Id, nA2.Id, Linkage.Follow);
                neo4j.Link<NodeA, NodeA>(nA1.Id, nA3.Id, Linkage.Follow);
                neo4j.Link<NodeA, NodeA>(nA2.Id, nA3.Id, Linkage.Follow);
                dict = neo4j.GetNodesLinks<NodeA, NodeA>(Linkage.Follow);
            }
            bool result = dict[nA1].Contains(nA2) && dict[nA2].Contains(nA3);
            Assert.AreEqual(result, true);
        }
        [TestMethod]
        public void TestGetNodeNotLinksOnQuery()
        {
            List<NodeA> list;
            Init();
            using (Neo4jConnector neo4j = new Neo4jConnector())
            {
                neo4j.Delete<NodeA>();
                neo4j.Create(nA1);
                neo4j.Create(nA2);
                neo4j.Create(nA3);
                neo4j.Create(nA4);

                neo4j.Link<NodeA, NodeA>(nA2.Id, nA1.Id, Linkage.Follow);
                neo4j.Link<NodeA, NodeA>(nA3.Id, nA1.Id, Linkage.Follow);
                neo4j.Link<NodeA, NodeA>(nA4.Id, nA1.Id, Linkage.Follow);
                neo4j.Link<NodeA, NodeA>(nA1.Id, nA4.Id, Linkage.Block);
                var results = neo4j.GetNodeLinkedByResults<NodeA, NodeA>(nA1.Id, Linkage.Follow);
                list = neo4j.GetNodeNotLinks<NodeA, NodeA>(results, "linker as node1,linkedBy as node2", nA1.Id, Linkage.Block);
            }

            bool result = !list.Contains(nA4) && list.Contains(nA2) && list.Contains(nA3);
            Assert.AreEqual(result, true);
        }


        [TestMethod]
        public void TestCreatedAndLinked()
        {
            
            Init();
            NodeA result;
            using (Neo4jConnector neo4j = new Neo4jConnector())
            {
                neo4j.Create(nA1);
                 result=neo4j.CreateAndLink<NodeA,NodeA>(nA1.Id, nA2, Linkage.Follow);
                
                
            }
             
            Assert.AreEqual(result, nA2);
        }
    }
}
