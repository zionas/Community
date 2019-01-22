using CommunityNetwork.Common.Inerfaces;
using CommunityNetwork.Common.Models;
using CommunityNetWork.Common.Enums;
using CommunityNetWork.Dal.Interfaces;
using Neo4jClient;
using Neo4jClient.Cypher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static CommunityNetWork.Dal.Neo4JFormat;
using static CommunityNetWork.Dal.Neo4JLambda;


namespace CommunityNetWork.Dal
{
    
    
    
    
    public class Neo4jConnector:IGraph
    {
        
        GraphClient _graph;
        string _user = "neo4j";
        string _password = "omer2803";
        string _uri = "http://ec2-18-217-80-168.us-east-2.compute.amazonaws.com:7474/db/data";
        

        public Neo4jConnector(bool useLocal = true)
        {
                        
            if (useLocal)
            {
               _password = "zzneo4j";
               _uri = "http://localhost:7474/db/data";
            }
           _graph = new GraphClient(new Uri(_uri), _user, _password);
        }
    
        GraphClient Connect()
        {
            _graph.Connect();
            return _graph;
        }
        

        public void Dispose()
        {
            _graph.Dispose();
        }
                
                
        static ICypherFluentQuery WhereQuery<TNode>(ICypherFluentQuery query, List<Tuple<string, object>> valuesToFind) where TNode : INode
        {
            valuesToFind.ForEach(keyValue =>
            {
                var lambda = CreatePropertyEqualsLambda<TNode>("node",keyValue.Item1, keyValue.Item2);
                query = query.Where(lambda);
            });
            return query;
        }
        
        

        public string GetModelName(string id) 
        {
            
            var query = CreatePropertyEqualsLambda<MNode>("node", "Id", id);
            return Connect().Cypher.Match("(node)")
                .Where(query)
                 .Return(node => node.Labels().ElementAt(0))
                 .Results.FirstOrDefault();

        }

        public TNode Get<TNode>(string id) where TNode : INode
        {
            string typeName = TypeName<TNode>();
            var query = CreatePropertyEqualsLambda<TNode>("node", "Id", id);
            return Connect().Cypher.Match("(node:" + typeName + ")")
                .Where(query)
                 .Return(node => node.As<TNode>())
                 .Results.FirstOrDefault();

        }
        
        public List<TNode> Get<TNode>() where TNode : INode
        {
            string typeName = TypeName<TNode>();
            
            return Connect().Cypher.Match("(node:" + typeName + ")")
                 .Return(node => node.As<TNode>())
                 .Results.ToList();

        }
        
        public List<TNode> Get<TNode>(string propertyName,object value) where TNode : INode
        {
            string typeName = TypeName<TNode>();
            var query = CreatePropertyEqualsLambda<TNode>("node",propertyName, value);
            return Connect().Cypher.Match("(node:" + typeName + ")")
                .Where(query)
                 .Return(node => node.As<TNode>())
                 .Results.ToList();
        }

        private static void CreateLinkage(ICypherFluentQuery query, Linkage linkage)
        {
            query.CreateUnique(GetLinkageString(linkage)).ExecuteWithoutResults();
        }
       

        private static List<Tuple<string, object>> GetFindByIdParams(params INode[] nodes)
        {
            List<Tuple<string, object>> valuesToFind = new List<Tuple<string, object>>();
            foreach (INode n in nodes)
            {
                valuesToFind.Add(new Tuple<string, object>("id", n.Id));

            }
            return valuesToFind;
        }
        

        public TNode Create<TNode>(TNode newNode) where TNode:INode
        {
            newNode.ResetId();
            string create = GetCreateString<TNode>();
            return Connect().Cypher
                .Create(create)
                .WithParam("newNode", newNode)
                .Return(node => node.As<TNode>())
                 .Results.FirstOrDefault();
        }
        

        public TNode Put<TNode>(TNode node) where TNode:INode
        {
            string merge = string.Format("(n:{0}", node.GetType().Name) + "{Id:{id}})";
            return Connect().Cypher
           .Merge(merge)
           .OnCreate()
           .Set("n = {node}")
           .WithParams(new
           {
               id = node.Id,
               node
           })
           .Return(n => n.As<TNode>())
                 .Results.FirstOrDefault();
        }
        public bool Delete(ICypherFluentQuery query)
        {
            
                query
            .DetachDelete("node")
            .ExecuteWithoutResults();
            
            return true;

        }
        public bool Delete<TNode>(string id) where TNode : INode
        {
            string typeName = TypeName<TNode>();
            string match = GetMatchDetachString<TNode>();
            var filter = CreatePropertyEqualsLambda<TNode>("node", "Id", id);

            var query = Connect().Cypher.Match(match)
                .Where(filter);
            Delete(query);
                
            return true;

        }


        public bool Delete<TNode>() where TNode : INode
        {
            string typeName = TypeName<TNode>();
            string match = GetMatchDetachString<TNode>();

            var query = Connect().Cypher.Match(match);
            Delete(query);
            return true;

        }

        public bool DeleteAll() 
        {
            var query = Connect().Cypher.Match("(node)")
                .With("(node) limit 1000");
            Delete(query);

            return true;

        }

        public bool IsLinkerOfLinkedBy<TLinkedBy,TLinker>(string linkedById,string linkerId,Linkage linkage)
            where TLinkedBy:INode where TLinker:INode
        {
            var linkMatch = GetMatchFullDefinedLinkageString<TLinkedBy, TLinker>(linkage);
            var findLinkedBy = CreateWhereEqualsLambda<TLinkedBy>("linkedBy", "Id",linkedById);
            var findLinker = CreateWhereEqualsLambda<TLinker>("linker", "Id",linkerId);

            var links = Connect().Cypher
               .Match(linkMatch)
               .Where(findLinkedBy)
               .AndWhere(findLinker)
               .Return(r => r.As<string>())
               .Results
               .Count();
            return links>0;
       }

        public bool IsLinkedBy<TLinker, TLinkedBy>(string linkedById, string linkerId, Linkage linkage)
            where TLinker : INode where TLinkedBy : INode
        {
            var linkByMatch = GetMatchFullDefinedLinkageString<TLinkedBy, TLinker>(linkage);
            var findNode = CreateWhereEqualsLambda<TLinkedBy>("linker", "Id", linkedById);
            var findLinked = CreateWhereEqualsLambda<TLinkedBy>("linkedBy", "Id", linkerId);

            var linksBy = Connect().Cypher
               .Match(linkByMatch)
               .Where(findNode)
               .AndWhere(findLinked)
               .Return(r => r.As<string>())
               .Results
               .Count();
            return linksBy > 0;
        }

        public TNew CreateAndLink<TNew,TLinked>(string linkerId, TNew newNode, Linkage linkage) 
            where TNew : INode
            where TLinked :INode 
        {
            newNode.ResetId();
            var linkedMatch = GetMatchLabelString<TLinked>("linker");
            var link = GetLinkageString(linkage);
            var findLinker = CreateWhereEqualsLambda<TLinked>("linker", "Id", linkerId);
            string create = GetCreateString<TNew>();

            return Connect().Cypher
                .Create(create)
                .WithParam("newNode", newNode)
                .With("node")
                .Match(linkedMatch)
                .Where(findLinker)
               .Merge(link)
               .Return(node => node.As<TNew>())
                .Results.FirstOrDefault();
          
        }
 
        public bool Link<TNode, TLinked>(string linkedById, string linkerId, Linkage linkage) where TNode : INode where TLinked : INode
        {
            var nodeMatch = GetMatchLabelString<TNode>("linkedBy");
            var linkedMatch = GetMatchLabelString<TLinked>("linker");
            var link = GetDefinedLinkageString( linkage);
            var findNode = CreateWhereEqualsLambda<TNode>("linkedBy", "Id",linkedById);
            var findLinked = CreateWhereEqualsLambda<TLinked>("linker", "Id",linkerId);

            Connect().Cypher
               .Match(nodeMatch, linkedMatch)
               .Where(findNode)
               .AndWhere(findLinked)
               .Merge(link)
               .OnCreate()
               .Set(" r.TimeStamp=timestamp()")
               .ExecuteWithoutResults();
            return true;
        }

        public bool UnLink<TNode, TLinked>(string linkedById, string linkerId, Linkage linkage) where TNode : INode where TLinked : INode
        {
            
            var linkMatch = GetMatchFullDefinedLinkageString<TNode,TLinked>(linkage);
            
            var findNode = CreateWhereEqualsLambda<TNode>("linkedBy", "Id", linkedById);
            var findLinked = CreateWhereEqualsLambda<TLinked>("linker", "Id", linkerId);

            Connect().Cypher
               .Match(linkMatch)
               .Where(findNode)
               .AndWhere(findLinked)
               .Delete("r")
               .ExecuteWithoutResults();
            return true;
        }

        public List<TResult> ReturnResults<TResult>(ICypherFluentQuery query, string tag) where TResult : INode
        {
            var returnLambda = CreateGetMethodLambda<TResult>(tag, "As");
            var orderBy = tag + ".CreateTime DESC";
            return query
                .Return(returnLambda)
            .OrderBy(orderBy)
            .Results.ToList();
        }

        public List<TLinker> GetNodeLinkers<TLinkedBy, TLinker>(
            string linkedById, 
            Linkage linkage
            )
            where TLinkedBy : INode
            where TLinker : INode
        {
            var filter = CreatePropertyEqualsLambda<TLinkedBy>(LinkedByTag,"Id", linkedById);
            string match = GetMatchFullDefinedLinkageString<TLinkedBy, TLinker>(linkage);

            var query = Connect().Cypher
            .Match(match)
            .Where(filter);
            return ReturnResults<TLinker>(query, LinkerTag);
            
        }

       public ICypherFluentQuery GetNodeLinkersQuery<TLinkedBy, TLinker>(
            string linkedById,
            Linkage linkage
            )
            where TLinkedBy : INode
            where TLinker : INode
        {
            var filter = CreatePropertyEqualsLambda<TLinkedBy>(LinkedByTag, "Id", linkedById);
            string match = GetMatchFullDefinedLinkageString<TLinkedBy, TLinker>(linkage);

            return Connect().Cypher
            .Match(match)
            .Where(filter);
        }
           
        
        public List<TNotLinked> GetNodeNotLinks<TNode, TNotLinked>(
            string linkedById,
            Linkage linkage
            )
            where TNode : INode
            where TNotLinked : INode
        {
            var findNode = CreatePropertyEqualsLambda<TNode>("node1","Id", linkedById);
           
            string match = GetMatchLabelsString<TNode>("node1","node2");
            var noLinkage = GetNoLinkageString(linkage);
            var query = Connect().Cypher
            .OptionalMatch(match)
            .Where(findNode)
            .AndWhere(noLinkage);

            return ReturnResults<TNotLinked>(query, "node2");
            
        }
        public ICypherFluentQuery GetNodeNotLinksQuery<TNode, TNotLinked>(
           ICypherFluentQuery query,
           string withTagsAs,
           Linkage linkage
           )
           where TNode : INode
           where TNotLinked : INode
        {
            
            var noLinkage = GetNoLinkageString(linkage);
             return query
            .With(withTagsAs)
            .Where(noLinkage);
        }

        public List<TWithoutLinks> GetNodeNotLinks<TNode, TWithoutLinks>(
            ICypherFluentQuery query,
            string withTagsAsNewTags,
            string nodeId,
            Linkage linkage
            )
            where TNode : INode
            where TWithoutLinks : INode
        {
            query = GetNodeNotLinksQuery<TNode, TWithoutLinks>(query, withTagsAsNewTags,  linkage);
            return ReturnResults<TWithoutLinks>(query, "node2");
        
        }
        
       
        public ICypherFluentQuery GetNodeLinkedByQuery<TLinkedBy, TLinker>(string linkerId, Linkage linkage
            )
            where TLinkedBy : INode
            where TLinker : INode
        {
            var returnLambda = CreateGetMethodLambda<TLinkedBy>(LinkerTag, "As");
            var filter = CreateWhereEqualsLambda<TLinker>(LinkerTag, "Id", linkerId);
            string match = GetMatchFullDefinedLinkageString<TLinkedBy, TLinker>(linkage);

            return Connect().Cypher
            .Match(match)
            .Where(filter);

        }
        public List<TLinkedBy> GetNodeLinkedBy<TLinkedBy, TLinker>(
                    string linkerId,
                    Linkage linkage
                    )
                    where TLinkedBy : INode
                    where TLinker : INode
        {
            
            var query = GetNodeLinkedByQuery<TLinkedBy, TLinker>(linkerId, linkage);
            return ReturnResults<TLinkedBy>(query, LinkedByTag);
            
        }
        

        public List<TLinkedByLinkedBy> GetNodeLinkedByLinkedBy <TLinker,TLinkedBy,TLinkedByLinkedBy>(
            string linkerId,
            Linkage linkage,
            Linkage linkageBy) 
            where TLinker : INode where TLinkedBy:INode where TLinkedByLinkedBy:INode

        {
            
            var query = GetNodeLinkedByLinkedByQuery<TLinkedByLinkedBy, TLinkedBy, TLinker>(linkerId, linkage, linkageBy);
            return ReturnResults<TLinkedByLinkedBy>(query, LinkedByLinkedByTag);
        
        }

        public ICypherFluentQuery GetNodeLinkedByLinkedByQuery <TLinkedByLinkedBy, TLinkedBy,TLinker >(
            string linkerId,
            Linkage linkage,
            Linkage linkageBy)
            where TLinker : INode where TLinkedBy : INode where TLinkedByLinkedBy : INode

        {
            var filter = CreatePropertyEqualsLambda<TLinker>(LinkerTag, "Id", linkerId);

            string match = GetMatchByLinkageByLinkageString<TLinkedByLinkedBy, TLinkedBy, TLinkedByLinkedBy>(linkage, linkageBy);
            return Connect().Cypher
            .OptionalMatch(match)
            .Where(filter);

        }

        public List<TLinkedByLinkedBy> GetNodeLinkedByNotLinkedBy<TLinker, TLinkedBy, TLinkedByLinkedBy>(
            string linkerId,
            Linkage linkage,
            Linkage linkageBy)
            where TLinker : INode where TLinkedBy : INode where TLinkedByLinkedBy : INode

        {
            var findNode = CreatePropertyEqualsLambda<TLinker>(LinkerTag,"Id", linkerId);
            var noLinkage = GetNoLinkageString(linkage);
            string match = GetMatchByLinkageByLinkageString<TLinker, TLinkedBy, TLinkedByLinkedBy>(linkage, linkageBy);
            var query = Connect().Cypher
            .OptionalMatch(match)
            .Where(findNode)
            .AndWhere(noLinkage);

            return ReturnResults<TLinkedByLinkedBy>(query, LinkedByLinkedByTag);
       
        }

        public List<TLinkedBy> GetNodesWithLinkersCount<TLinkedBy,TLinker>(ICypherFluentQuery query, Linkage linkage, string tag)
            where TLinkedBy:INode
            where TLinker: INode
        {
             query = GetNodesWithLinkersCountQuery<TLinkedBy, TLinker>(query, linkage, tag);
            return ReturnResults<TLinkedBy>(query, tag);
             
        }

        public ICypherFluentQuery GetNodesWithLinkersCountQuery<TLinkedBy, TLinker>(ICypherFluentQuery query, Linkage linkage, string tag)
            where TLinkedBy : INode
            where TLinker:INode
        {
            string countPropertyName = linkage.ToString() + "s";
            string match = tag == LinkedByTag ? GetMatchLinkedByUndefinedLinkageString<TLinkedBy, TLinker>(linkage)
                : GetMatchLinkedByLinkedByUndefinedLinkageString<TLinkedBy, TLinker>(linkage);

            var withLinkersCount = string.Format("{0}, size(" + match + ") as {3}", tag, linkage.ToString(), TypeName<TLinkedBy>(), countPropertyName);
            var setCount = string.Format("{0}.{1}={1}", tag, countPropertyName);
            var orderBy = tag + ".CreateTime DESC";
            return query
            .With(withLinkersCount)
            .Set(setCount);
            
        }

        public List<TLinkedBy> GetNodesWithLinkers<TLinkedBy, TLinker>(ICypherFluentQuery query, Linkage linkage,string with)
        where TLinkedBy : INode where TLinker : INode
        {

            string match = GetMatchFullUndefinedLinkageString<TLinkedBy, TLinker>(linkage);
            
             return query
             .With(with)
            .Match(match)
            .Return((linkedBy, linker) => new
            {
                Node = linkedBy.As<TLinkedBy>(),
                Links = linker.CollectAs<TLinker>()
            })
            .Results
            .AsParallel()
            .Select(x=>
            {
                var node = x.Node;
                node.GetType().GetProperty(linkage.ToString() + "rs").SetValue(node, x.Links);
                return node;
            })
            .ToList();
                   
        }


        public List<Post> GetVisiblePosts(string profileId,int skip)
        {
            string matchId = string.Format("(profile:Profile{{Id:{0} }})", profileId);
            string matchPublicPost = "(post:Post{IsPublic:true })";
            string matchPost = "(post:Post)";
            var query = Connect().Cypher
                .Match(matchId + "<-[:Follow]->(p:Profile))-[:Publish]->" + matchPost)
                .Return<Post>("post")
                .Union()
                .Match(matchId + "-[:Follow]->(p:Profile))-[:Publish]->" + matchPublicPost)
                .Return<Post>("post")
                .Union()
                .OptionalMatch(matchId + "<-[:Recommended]-" + matchPost + "<-[:Publish]-(publisher:Profile)")
                .Where("not" + matchId +"-[:Block]-"+ "(publisher: Profile)")
                .Return<Post>("post")
                .Skip(skip)
                .Limit(10);
            return ReturnResults<Post>(query, "post");
                
                
        }
        ////////////////////////////////////////////////////////
        ///to rewrite
        ///
        /*public Dictionary<TNode, List<TLinks>> ReturnNodesResults<TNode, TLinks>(ICypherFluentQuery query, string nodeTag)
           where TNode : INode
           where TLinks : INode
        {
            var returned = nodeTag == "linker" ?
              query
             .Return((linker, linkedBy) => new
             {
                 Node = linker.As<TNode>(),
                 LinksCollection = linkedBy.CollectAs<TLinks>()
             })
             :
             query
             .Return((linker, linkedBy) => new
             {
                 Node = linkedBy.As<TNode>(),
                 LinksCollection = linker.CollectAs<TLinks>()
             });

            var results = returned
            .Results.ToList();

            Dictionary<TNode, List<TLinks>> dict = new Dictionary<TNode, List<TLinks>>();
            results.ForEach(x => dict.Add(x.Node, x.LinksCollection.ToList()));
            return dict;
        }

        public Dictionary<TNode, List<TLinks>> GetNodesLinks<TNode, TLinks>(string linkerId,
         Linkage linkage,
         string nodeTag)

           where TNode : INode where TLinks : INode
        {
            var where = CreatePropertyEqualsLambda<TNode>(nodeTag, "Id", linkerId);
            string match = nodeTag == LinkerTag ? GetMatchFullUndefinedLinkageString<TLinks, TNode>(linkage)
              : GetMatchFullUndefinedLinkageString<TNode, TLinks>(linkage);

            var query = Connect().Cypher
            .OptionalMatch(match)
            .Where(where);
            return ReturnNodesResults<TNode, TLinks>(query, nodeTag);


        }

        public Dictionary<TLinker, List<TLinkedBy>> GetNodesLinkedBy<TLinker, TLinkedBy>(string linkerId,
          Linkage linkage)

            where TLinker : INode where TLinkedBy : INode
        {
            return GetNodesLinks<TLinker, TLinkedBy>(linkerId, linkage, LinkerTag);
        }

        public Dictionary<TLinker, List<TLinkedBy>> GetNodesLinkers<TLinker, TLinkedBy>(string linkerId,
          Linkage linkage)

            where TLinker : INode where TLinkedBy : INode
        {
            return GetNodesLinks<TLinker, TLinkedBy>(linkerId, linkage, LinkedByTag);
        }
        TNode Find<TNode>(List<Tuple<string, object>> valuesToFind) where TNode : INode
        {
            var query = Connect().Cypher.Match(GetMatchDefinedLabelString<TNode>());
            return WhereQuery<TNode>(query, valuesToFind)
                 .Return(node => node.As<TNode>())
                 .Results
                 .Single();
        }

        TNode Find<TNode>(Expression<Func<TNode, bool>> lambda) where TNode : INode
        {
            return Connect().Cypher.Match(GetMatchDefinedLabelString<TNode>())
            .Where(lambda)
            .Return(node => node.As<TNode>())
            .Results.FirstOrDefault();

        }

        public List<TNode> GetNodesWithLinksCollections<TNode>(ICypherFluentQuery results, Linkage linkage, string tag)
            where TNode : INode
        {
            string linksSuffix = tag.EndsWith("er") ? "ers" : "s";
            string collectionPropertyName = linkage.ToString() + linksSuffix;
            var with = string.Format("{0}, collect(({0}) -[:{1}]->(: {2})) as {3}", tag, linkage.ToString(), TypeName<TNode>(), collectionPropertyName);

            var setCount = string.Format("{0}.{1}={1}", tag, collectionPropertyName);
            return results
            .With(with)
            .Set(setCount)
            .Return(CreateGetMethodLambda<TNode>(tag, "As"))
            .Results.ToList();
        }

        public bool Delete<TNode>(string id) where TNode : INode
        {
            string typeName = TypeName<TNode>();
            string match = GetMatchDeleteLabelString<TNode>();
            var query = CreatePropertyEqualsLambda<TNode>("linkednode", "Id", id);
            try
            {
                var count = Connect().Cypher.Match(match)
                .Where(query)
                 .Return(node => node.As<TNode>()).Results.Count();
                Console.WriteLine("removing {0} {1}s", count, typeName);
                Connect().Cypher.Match("(linkednode:" + typeName + ")")
                .Where(query)
                .Delete("r,linkednode")
                .ExecuteWithoutResults();
            }
            catch (Exception e)
            {
                throw e;
            }

            return true;

        }*/
    }
}
