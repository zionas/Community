using System;
using System.Collections.Generic;
using CommunityNetwork.Common;
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
        public int Likes { get; set; }
        public List<NodeA> Likers { get; set; }
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
        Profile p1, p2;
        public void Init()
        {
            p1 = new Profile
            {
                UserName = "p1",
                
            };
            p2 = new Profile
            {
                UserName = "p2",
                
            };

            nA1 = new NodeA
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
        public void TestDeleteAllNodes()
        {
            NodeA node;
            Profile p;
           
            bool createNodeA,createP, deleteAll;
            Init();
            using (Neo4jConnector neo4j = new Neo4jConnector())
            {
                node = neo4j.Create(nA1);
                p = neo4j.Create(p1);
                
                createNodeA = neo4j.Get<NodeA>().Count>0;
                createP= neo4j.Get<Profile>().Count > 0;
                neo4j.DeleteAll();
                deleteAll = neo4j.Get<NodeA>().Count == 0
                    && neo4j.Get<Profile>().Count == 0;
            
            }
            bool result = createNodeA&& createP&&deleteAll; ;
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
                result = neo4j.IsLinkerOfLinkedBy<NodeA, NodeA>(nA1.Id, nA2.Id, Linkage.Follow);
                
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
        public void TestGetNodesWithLikersCount()
        {
            List<NodeA> list;
            Init();
            using (Neo4jConnector neo4j = new Neo4jConnector())
            {
                neo4j.Create(nA1);
                neo4j.Create(nA2);
                neo4j.Create(nA3);
                Linkage linkage = Linkage.Like;
                neo4j.Link<NodeA, NodeA>(nA1.Id, nA2.Id, linkage);
                neo4j.Link<NodeA, NodeA>(nA1.Id, nA3.Id, linkage);
                var results = neo4j.GetNodeLinkedByQuery<NodeA, NodeA>(nA2.Id, Linkage.Like);
                list = neo4j.GetNodesWithLinkersCount<NodeA,NodeA>(results, linkage, "linkedBy");
            }
            bool result = list.Contains(nA1) && list[0].Likes==2;
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void TestGetFollowedFollowdNodesWithLikersCount()
        {
            List<NodeA> list;
            Init();
            using (Neo4jConnector neo4j = new Neo4jConnector())
            {
                NodeA post = nA1,author=nA2,follower=nA3;
                neo4j.Create(nA1);
                neo4j.Create(nA2);
                neo4j.Create(nA3);
                Linkage like = Linkage.Like;
                Linkage follow = Linkage.Follow;
                Linkage publish = Linkage.Publish;
                neo4j.Link<NodeA, NodeA>(post.Id, nA2.Id, publish);
                neo4j.Link<NodeA, NodeA>(author.Id, follower.Id, follow);
                neo4j.Link<NodeA, NodeA>(post.Id, follower.Id, like);
                neo4j.Link<NodeA, NodeA>(post.Id, author.Id, like);
                var results = neo4j.GetNodeLinkedByLinkedByQuery<NodeA, NodeA,NodeA>(follower.Id, follow,publish);
                list = neo4j.GetNodesWithLinkersCount<NodeA, NodeA>(results, like, "linkedByLinkedBy");
            }
            bool result = list.Contains(nA1) && list[0].Likes == 2;
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void TestGetFollowedFollowdNodesWithLikersCollection()
        {
            List<NodeA> list;
            Init();
            using (Neo4jConnector neo4j = new Neo4jConnector())
            {
                NodeA post = nA1, author = nA2, follower = nA3;
                neo4j.Create(nA1);
                neo4j.Create(nA2);
                neo4j.Create(nA3);
                Linkage like = Linkage.Like;
                Linkage follow = Linkage.Follow;
                Linkage publish = Linkage.Publish;
                neo4j.Link<NodeA, NodeA>(post.Id, nA2.Id, publish);
                neo4j.Link<NodeA, NodeA>(author.Id, follower.Id, follow);
                neo4j.Link<NodeA, NodeA>(post.Id, follower.Id, like);
                neo4j.Link<NodeA, NodeA>(post.Id, author.Id, like);
                var results = neo4j.GetNodeLinkedByLinkedByQuery<NodeA, NodeA, NodeA>(follower.Id, follow, publish);
                list = neo4j.GetNodesWithLinkers<NodeA,NodeA>(results,like,"linkedByLinkedBy as linkedBy");
            }
            bool result = list.Contains(nA1) && list[0].Likers.Count == 2;
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
                neo4j.Create(p1);
                neo4j.Create(p2);
                neo4j.Link<Profile,Profile>(p1.Id, p2.Id, Linkage.Follow);
                neo4j.Link<NodeA, NodeA>(nA1.Id, nA2.Id, Linkage.Follow);
                neo4j.Link<NodeA, NodeA>(nA1.Id, nA3.Id, Linkage.Follow);
                list = neo4j.GetNodeLinkers<NodeA, NodeA>(nA1.Id, Linkage.Follow);
                List<Profile> list1= neo4j.GetNodeLinkers<Profile, Profile>("id1", Linkage.Recommended);
            }
            bool result = list.Contains(nA2) && list.Contains(nA3);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void TestGetProfileLinkers()
        {
            List<Profile> list;
            Init();
            using (Neo4jConnector neo4j = new Neo4jConnector())
            {

                neo4j.Delete<Profile>();
                neo4j.Create(p1);
                neo4j.Create(p2);
                neo4j.Link<Profile, Profile>(p1.Id, p2.Id, Linkage.Follow);
                
                list = neo4j.GetNodeLinkers<Profile,Profile>(p1.Id, Linkage.Follow);
            }
            bool result = list.Contains(p2);
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
                var results = neo4j.GetNodeLinkedByQuery<NodeA, NodeA>(nA1.Id, Linkage.Follow);
                list = neo4j.GetNodeNotLinks<NodeA, NodeA>(results, "linker as node1,linkedBy as node2", nA1.Id, Linkage.Block);
            }

            bool result = !list.Contains(nA4) && list.Contains(nA2) && list.Contains(nA3);
            Assert.AreEqual(result, true);
        }



        [TestMethod]
        public void TestCreatedAndLinkLikePost()
        {

            //Init();
            Profile result;//NodeA result;// 
            Profile pf = new Profile
            {
                UserName = "dan"
            };
            Post post = new Post
            {

            };
            using (Neo4jConnector neo4j = new Neo4jConnector())
            {
                neo4j.Create( post);
                result = neo4j.CreateAndLink<Profile, Post>(post.Id, pf, Linkage.Like);//<NodeA, NodeA>(nA1.Id, nA2, Linkage.Like);//


            }

            Assert.AreEqual(result, pf);
        }
        [TestMethod]
        public void TestCreatedAndLinkforSpark()
        {
            bool result;


            using (Neo4jConnector neo4j = new Neo4jConnector())
            {
                List<Profile> profiles1 = new List<Profile>();
                List<Profile> profiles2 = new List<Profile>();
                List<Post> posts = new List<Post>();
                neo4j.DeleteAll();
                
                for (int i=0; i < 22;i++)
                {
                    Profile p = new Profile
                    {
                        UserName = "user" + i,
                        Id = "id"+i
                    };
                    Post po = new Post
                    {
                        Id = "postId"+i
                    };
                    profiles1.Add(p);
                    profiles2.Add(p);
                    posts.Add(po);
                    neo4j.Create(p);
                    neo4j.Create(po);

                }
                
                for(int i = 0; i < 22; i++)
                {
                    neo4j.Link<Post, Profile>(posts[i].Id, profiles2[i].Id, Linkage.Publish);
                    for (int j = 0; j < 22; j++)
                    {
                        if (i != j)
                        {
                            if (i % 2 == 0 && j % 2 != 0 || i % 2 != 0 && j % 2 == 0)
                            {
                                neo4j.Link<Profile, Profile>(profiles1[j].Id, profiles2[i].Id, Linkage.Follow);
                            }
                           
                        }
                        if (i % 2 == 0)
                        {
                            neo4j.Link<Post, Profile>( posts[i].Id, profiles1[j].Id, Linkage.Like);
                        }
                            

                    }
                        
                }
                var list1 = neo4j.GetNodeLinkedBy<Profile, Profile>("id1", Linkage.Follow);
                var list2= neo4j.GetNodeLinkedBy<Post, Profile>("id2", Linkage.Like);

                result =  list1.Count ==11&&list2.Count==11;

            }

            Assert.AreEqual(result, true);
        }
    }
}
