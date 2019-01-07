﻿using System;
using System.Collections.Generic;

using CommunityNetWork.Dal;
using CommunityNetWork.Dal.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Neo4J.Test
{
    
     class NodeA : Node
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
        NodeA nA1,nA2,nA3;
        public void Init()
        {
            nA1= new NodeA
            {
                Id = Guid.NewGuid(),
                IntProperty = 1,
                StringProperty = "1"


            };
            nA2 = new NodeA
            {
                Id = Guid.NewGuid(),
                IntProperty = 2,
                StringProperty = "2"


            };
            nA3 = new NodeA
            {
                Id = Guid.NewGuid(),
                IntProperty = 3,
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
        public void TestLinkNode()
        {
            bool result;
            Init();
            using (Neo4jConnector neo4j = new Neo4jConnector())
            {
                neo4j.Create(nA1);
                neo4j.Create(nA2);
                result =neo4j.Link<NodeA, NodeA>(nA1.Id, nA2.Id, Linkage.Follow);
                
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
                list = neo4j.GetNodeLinks<NodeA,NodeA>(nA1.Id, Linkage.Follow);
            }
            bool result = list.Contains(nA2)&& list.Contains(nA3);
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
        public void TestCreatedAndLinked()
        {
            
            Init();
            bool result;
            using (Neo4jConnector neo4j = new Neo4jConnector())
            {
                neo4j.Create(nA1);
                 result=neo4j.CreateAndLink<NodeA,NodeA>(nA1.Id, nA2, Linkage.Follow);
                
                
            }
             
            Assert.AreEqual(result, true);
        }
    }
}