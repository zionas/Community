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
using System.Reflection;

namespace CommunityNetWork.Dal
{
    
    public class LinkParams
    {
        public LinkParams()
        {
            TimeStamp = DateTime.Now;
        }

        public DateTime TimeStamp { get; set; }
    }
    
    public class LinkedNode
    {
        public string NodeId { get; set; }
        public Linkage LinkageVal { get; set; }
        public LinkParams LinkParamVal { get; set; }
    }
    public class Neo4jConnector:IGraph
    {
        const string MatchLabelFormat = "(node:{0})";
        const string CreateFormat = "(node:{0}{{newNode}})";
        const string MatchDeleteLabelFormat = "(node:{0})-[r]-()";
        const string MatchOneDefinedLinkageFormat = "(node:{0})<-[r:{1}]-(linked:{2})";
        const string MatchByDefinedLinkageFormat = "(node:{0})-[r:{1}]->(linkedBy:{2})";
        const string MatchAnyDefinedLinkageFormat = "(node:{0})-[r:{1}]-(linkedBy:{2})";
        const string MatchByDefinedLinkageByLinkageFormat = "(node:{0})-[r1:{1}]->(linkedby:{2})-[r2:{3}]->(linkedbylinkedby:{4})";
        const string MatchOneLinkageFormat = "(node:{0})<-[:{1}]-(linked:{2})";
        const string MatchByLinkageFormat = "(node:{0})-[:{1}]->(linked:{2})";
        const string MatchAnyLinkageFormat = "(node)-[:{0}]-(linked)";
        const string MatchByLinkageByLinkageFormat = "(node:{0})-[:{1}]->(linkedby:{2})-[:{3}]->(linkedbylinkedby:{4})";
        const string MatchStringLinkageFormat = "{0}{1}[r:{2}]{3}(linked:{4})";
        const string ByLinkageFormat = "(node)-[:{0}]->(linkedBy)";
        const string CreateAndLinkFormat = "(linked:{0})-[:{1}]->(node:{2}{{newNode}})";
        const string CreateAndLinkWithParamsFormat = "(linked:{0})-[:{0}{{relParam}}]->(node:{1}{{newNode}})";
        
        static readonly string AnyLinkageFormat = "(node1)<-[:{0}]-(node2)";
        static readonly string OneLinkageFormat = "(node)<-[:{0}]-(linked)";
        static readonly string OneLinkageWithParamsFormat = "(node)<-[:{0} {{relParam}}]-(linked)";
        static readonly string ByLinkageWithParamsFormat = "(node)-[:{0} {{relParam}}]->(linkedBy)";

        GraphClient _graph;
        string _user = "neo4j";
        string _password = "omer2803";//"zzneo4j";
        string _uri; /*"http://ec2-18-217-80-168.us-east-2.compute.amazonaws.com:7474";*/
        

        public Neo4jConnector(bool useLocal = true)
                             
        {

            
            if (useLocal)
            {
                _password = "zzneo4j";
                _uri = "http://localhost:7474/db/data";
            }
            
           
            _graph = new GraphClient(new Uri(_uri), _user, _password);
        }
        
        
        static string TypeName<T>()
        {
            return typeof(T).Name;

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
        

        static Expression<Func<TNode, bool>> CreateWhereEqualsLambda<TNode>(string PropertyName, object value) where TNode : INode
        {
            ParameterExpression x = Expression.Parameter(typeof(TNode), "node");
            Expression statusCode = Expression.Property(x, PropertyName);
            Expression comparison = Expression.Equal(statusCode, Expression.Constant(value));
            return Expression.Lambda<Func<TNode, bool>>(comparison, x);
        }
        

        static Expression<Func<LinkParams, bool>> CreateWhereNewerLambda<LinkParams>(object value)
        {
            ParameterExpression x = Expression.Parameter(typeof(LinkParams), "r");
            Expression statusCode = Expression.Property(x, "DateTime");
            Expression comparison = Expression.GreaterThanOrEqual(statusCode, Expression.Constant(value));
            return Expression.Lambda<Func<LinkParams, bool>>(comparison, x);
        }
        
        static Expression<Func<TNode, bool>> CreateWhereEqualsLambda<TNode>(string param, string propertyName, object value) where TNode : INode
        {
            
                ParameterExpression x = Expression.Parameter(typeof(TNode), param);
            Expression statusCode = Expression.Property(x, propertyName);
            Expression comparison = Expression.Equal(statusCode, Expression.Constant(value));
            return Expression.Lambda<Func<TNode, bool>>(comparison, x);
        }
        

        static ICypherFluentQuery WhereQuery<TNode>(ICypherFluentQuery query, List<Tuple<string, object>> valuesToFind) where TNode : INode
        {
            valuesToFind.ForEach(keyValue =>
            {
                var lambda = CreateWhereEqualsLambda<TNode>(keyValue.Item1, keyValue.Item2);
                query = query.Where(lambda);
            });
            return query;
        }
        

        TNode Find<TNode>(List<Tuple<string, object>> valuesToFind) where TNode : INode
        {
            var query = Connect().Cypher.Match(GetMatchLabelString<TNode>());
            return WhereQuery<TNode>(query, valuesToFind)
                 .Return(node => node.As<TNode>())
                 .Results
                 .Single();
        }
        
        TNode Find<TNode>(Expression<Func<TNode, bool>> lambda) where TNode : INode
        {
            return Connect().Cypher.Match(GetMatchLabelString<TNode>())
            .Where(lambda)
            .Return(node => node.As<TNode>())
            .Results.FirstOrDefault();

        }
        

        public TNode Get<TNode>(string id) where TNode : INode
        {
            string typeName = TypeName<TNode>();
            var query = CreateWhereEqualsLambda<TNode>("Id", id);
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


        static string GetMatchLabelFormat(int counter)
        {
            string labelFormat = "";
            if (counter > 0)
                labelFormat += ",";
            labelFormat += "node" + (counter + 1) + ":{" + counter + "})";
            return labelFormat;
        }

        static string GetMatchLabelFormat(int counter,string tag)
        {
            string labelFormat = "";
            if (counter > 0)
                labelFormat += ",";
            labelFormat += "("+tag  + ":{" + counter + "})";
            return labelFormat;
        }
        static string GetMatchLabelString<TNode>(string tag)
        {
            string format = "({0}:{1})";
            return string.Format(format, tag, TypeName<TNode>());
        }
        

        static string GetMatchLabelString<TNode>()
        {

            string format = MatchLabelFormat ;
            return string.Format(format, TypeName<TNode>());
        }
        

        static string GetMatchLabelsString(params MNode[] nodes)
        {
            string[] nodesLabels = new string[nodes.Length];
            string format = "";//"(";
            for (int i = 0; i < nodes.Length; i++)
            {
                format += GetMatchLabelFormat(i);
                nodesLabels[i] = nodes[i].NodeName;
            }
            //format += ")";
            return string.Format(format, nodesLabels);
        }

        static string GetMatchLabelsString<TNode>(params string[] nodesTags)
            where TNode:INode
        {
            string[] nodesLabels = new string[nodesTags.Length];
            string format = "";
            for (int i = 0; i < nodesTags.Length; i++)
            {
                format += GetMatchLabelFormat(i,nodesTags[i]);
                nodesLabels[i] = TypeName<TNode>();
            }
            
            return string.Format(format, nodesLabels);
        }


        static string GetMatchLinkageString<TNode,TLinked>(Linkage linkage)
        {
            return string.Format(MatchOneDefinedLinkageFormat, TypeName<TNode>(),linkage,TypeName<TLinked>());
        }
        

        static string GetMatchByLinkageString<TNode, TLinkedBy>(Linkage linkage)
        {
            return string.Format(MatchByDefinedLinkageFormat, TypeName<TNode>(),linkage, TypeName<TLinkedBy>());
        }
        

        static string GetMatchByLinkageByLinkageString<TNode, TLinkedBy,TLinkedByLinkedBy>(Linkage linkage,Linkage linkageBy)
        {
            return string.Format(MatchByDefinedLinkageByLinkageFormat, TypeName<TNode>(),linkage, TypeName<TLinkedBy>(), linkageBy,TypeName<TLinkedByLinkedBy>());
        }
        


        static string GetCreateString<TNode>()
        {
            return string.Format(CreateFormat, TypeName<TNode>());
        }
        private static string GetOneLinkageString(Linkage linkage)
        {
            return string.Format(OneLinkageFormat, linkage);
        }
        static string GetMatchDeleteLabelString<TNode>()
        {
            return string.Format(MatchDeleteLabelFormat,TypeName<TNode>());
        }

        private static string GetMatchOneDefinedLinkageString<TNode, TLinked>(Linkage linkage)
        {
            return string.Format(MatchOneDefinedLinkageFormat, TypeName<TNode>(), linkage, TypeName<TLinked>());
        }


        private static string GetMatchAnyDefinedLinkageString<TNode, TLinked>(Linkage linkage)
        {
            return string.Format(MatchAnyDefinedLinkageFormat, TypeName<TNode>(), linkage, TypeName<TLinked>());
        }
        private static string GetMatchAnyLinkageString<TNode, TLinked>(Linkage linkage)
        {
            return string.Format(MatchAnyLinkageFormat, TypeName<TNode>(), linkage, TypeName<TLinked>());
        }


        private static string GetMatchAnyLinkageString(Linkage linkage)
        {
            return string.Format(MatchAnyLinkageFormat, linkage);
        }
        private static string GetMatchOneLinkageString<TNode, TLinked>(Linkage linkage)
        {
            return string.Format(MatchOneLinkageFormat, TypeName<TNode>(), linkage, TypeName<TLinked>());
        }

        private static string GetOneLinkageWithParamsString(Linkage linkage)
        {
            return string.Format(OneLinkageWithParamsFormat, linkage);
        }
        static string GetCreateAndLinkString<TLinked,TNew>(Linkage linkage)
        {
            return string.Format(CreateAndLinkFormat, TypeName < TLinked >(), linkage, TypeName<TNew>());
        }
        static string GetCreateAndLinkWithParamsString<TLinked, TNew>(Linkage linkage)
        {
            return string.Format(CreateAndLinkWithParamsFormat, TypeName<TLinked>(), linkage, TypeName<TNew>());
        }
        static void CreateByLinkageWithParam(ICypherFluentQuery query, Linkage linkage, LinkParams linkParams)
        {
           
            query.CreateUnique(string.Format(ByLinkageWithParamsFormat, linkage))
                    .WithParam("relParam", linkParams)
                    .ExecuteWithoutResults();
        }
        

        static void CreateLinkage(ICypherFluentQuery query, Linkage linkage)
        {
            query.CreateUnique(string.Format(OneLinkageFormat, linkage)).ExecuteWithoutResults();
        }
        

        static void CreateByLinkage(ICypherFluentQuery query, Linkage linkage)
        {
             query.CreateUnique(string.Format(ByLinkageFormat,  linkage)).ExecuteWithoutResults();
        }
        

        static List<Tuple<string, object>> GetFindByIdParams(params INode[] nodes)
        {
            List<Tuple<string, object>> valuesToFind = new List<Tuple<string, object>>();
            foreach (INode n in nodes)
            {
                valuesToFind.Add(new Tuple<string, object>("id", n.Id));

            }
            return valuesToFind;
        }
        

        public ICypherFluentQuery NodesQuery(params MNode[] nodes)
        {
            var findByIdParams = GetFindByIdParams(nodes);
            string matchLabelsString = GetMatchLabelsString(nodes);
            var query = Connect().Cypher.Match(matchLabelsString);
            return WhereQuery<INode>(query, findByIdParams);
        }
        

        public void LinkBy(MNode node, MNode linkedBy, Linkage linkage, LinkParams linkParams )
        {
            var query = NodesQuery(node, linkedBy);

            if (linkParams == null)
                CreateByLinkage(query, linkage);
            else
                CreateByLinkageWithParam(query, linkage, linkParams);
        }
        

        public static Expression<Func<ICypherResultItem, ICypherResultItem, ICypherResultItem, LinkedNode>> CreateLinkageResultLambda()
        {
            return (ICypherResultItem other, ICypherResultItem l, ICypherResultItem r) =>
                new LinkedNode
                {
                    NodeId = other.As<MNode>().Id,
                    LinkageVal = l.As<Linkage>(),
                    LinkParamVal = r.As<LinkParams>()
                };
        }
        

        public List<LinkedNode> GetLinksBy<TNode,TLinked>(MNode linkedBy, Linkage linkage)
            where TNode : INode
            where TLinked: INode
        {
            string match = GetMatchByLinkageString<TNode,TLinked>(linkage);

            var query = Connect().Cypher
                .Match(match)
                .Where((MNode n) => n.Id == linkedBy.Id)
                .Return(CreateLinkageResultLambda());

            return query.Results.ToList();
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
        

        public bool Delete<TNode>(string id) where TNode : INode
        {
            string typeName = TypeName<TNode>();
            string match = GetMatchDeleteLabelString<TNode>();
            var query = CreateWhereEqualsLambda<TNode>("Id", id);
            try
            {
                var count = Connect().Cypher.Match(match)
                .Where(query)
                 .Return(node => node.As<TNode>()).Results.Count();
                Console.WriteLine("removing {0} {1}s", count, typeName);
                Connect().Cypher.Match("(node:" + typeName + ")")
                .Where(query)
                .Delete("r,node")
                .ExecuteWithoutResults();
            }
            catch (Exception e)
            {
                throw e;
            }

            return true;

        }


        public bool Delete<TNode>() where TNode : INode
        {
            string typeName = TypeName<TNode>();
            string match = GetMatchDeleteLabelString<TNode>();

            try
            {
                /*var count = Connect().Cypher.Match("(node:" + typeName + ")")
                .Return(node => node.As<TNode>()).Results.Count();
                Console.WriteLine("removing {0} {1}s", count, typeName);*/
                Connect().Cypher.Match(match)
                .Delete("r,node")
                .ExecuteWithoutResults();
            }
            catch (Exception e)
            {
                throw e;
            }

            return true;

        }
        public bool IsLinked<TNode,TLinked>(string nodeId,string linkedId,Linkage linkage)
            where TNode:INode where TLinked:INode
        {
            var linkMatch = GetMatchLinkageString<TNode, TLinked>(linkage);
            var findNode = CreateWhereEqualsLambda<TNode>("node", "Id", nodeId);
            var findLinked = CreateWhereEqualsLambda<TLinked>("linked", "Id", linkedId);

            var links = Connect().Cypher
               .Match(linkMatch)
               .Where(findNode)
               .AndWhere(findLinked)
               .Return(r => r.As<string>())
               .Results
               .Count();
            return links>0;
       }

        public bool IsLinkedBy<TNode, TLinkedBy>(string nodeId, string linkedId, Linkage linkage)
            where TNode : INode where TLinkedBy : INode
        {
            var linkByMatch = GetMatchByLinkageString<TNode, TLinkedBy>(linkage);
            var findNode = CreateWhereEqualsLambda<TNode>("node", "Id", nodeId);
            var findLinked = CreateWhereEqualsLambda<TLinkedBy>("linkedby", "Id", linkedId);

            var linksBy = Connect().Cypher
               .Match(linkByMatch)
               .Where(findNode)
               .AndWhere(findLinked)
               .Return(r => r.As<string>())
               .Results
               .Count();
            return linksBy > 0;
        }

        public TNew CreateAndLink<TNew,TLinked>(string linkedId, TNew newNode, Linkage linkage) 
            where TNew : INode
            where TLinked :INode 
        {
            newNode.ResetId();
            var linkedMatch = GetMatchLabelString<TLinked>("linked");
            var link = GetOneLinkageString(linkage);
            var findLinked = CreateWhereEqualsLambda<TLinked>("linked", "Id", linkedId);
            string create = GetCreateString<TNew>();

            return Connect().Cypher
                .Create(create)
                .WithParam("newNode", newNode)
                .With("node")
                .Match(linkedMatch)
                .Where(findLinked)
               .Create(link)
               .Return(node => node.As<TNew>())
                .Results.FirstOrDefault();

           
        }
        

        public TNew CreateAndLinkWithParams<TNew,TLinked>(string linkedId, TNew newNode, Linkage linkage, LinkParams linkParams) 
            where TNew: INode
            where TLinked:INode
        {
           
            var linkedMatch = GetMatchLabelString<TLinked>("linked");
            var link = GetOneLinkageWithParamsString(linkage);
            var findLinked = CreateWhereEqualsLambda<TLinked>("linked", "Id", linkedId);
            string create = GetCreateString<TNew>();

            return Connect().Cypher
                .Create(create)
                .WithParam("newNode", newNode)
                .With("node")
                .Match(linkedMatch)
                .Where(findLinked)
                .Create(link)
                .WithParam("relParam", linkParams)
                .Return(node => node.As<TNew>())
                .Results.FirstOrDefault();

            
        }
        

        public bool Link<TNode, TLinked>(string nodeId, string linkedId, Linkage linkage) where TNode : INode where TLinked : INode
        {
            var nodeMatch = GetMatchLabelString<TNode>("node");
            var linkedMatch = GetMatchLabelString<TLinked>("linked");
            var link = GetOneLinkageString( linkage);
            var findNode = CreateWhereEqualsLambda<TNode>("node", "Id", nodeId);
            var findLinked = CreateWhereEqualsLambda<TLinked>("linked", "Id", linkedId);

            Connect().Cypher
               .Match(nodeMatch, linkedMatch)
               .Where(findNode)
               .AndWhere(findLinked)
               .Create(link)
               .ExecuteWithoutResults();
            return true;
        }

        
        public bool UnLink<TNode, TLinked>(string nodeId, string linkedId, Linkage linkage) where TNode : INode where TLinked : INode
        {
            
            var linkMatch = GetMatchLinkageString<TNode,TLinked>(linkage);
            
            var findNode = CreateWhereEqualsLambda<TNode>("node", "Id", nodeId);
            var findLinked = CreateWhereEqualsLambda<TLinked>("linked", "Id", linkedId);

            Connect().Cypher
               .Match(linkMatch)
               .Where(findNode)
               .AndWhere(findLinked)
               .Delete("r")
               .ExecuteWithoutResults();
            return true;
        }
        

        public bool LinkWithParams<TNode, TLinked>(string nodeId, string linkedId, Linkage linkage, LinkParams linkParams) where TNode : INode where TLinked : INode
        {
            var nodeMatch = GetMatchLabelString<TNode>("node");
            var linkedMatch = GetMatchLabelString<TLinked>("linked");
            var link = string.Format(OneLinkageWithParamsFormat, linkage);
            var findNode = CreateWhereEqualsLambda<TNode>("node", "Id", nodeId);
            var findLinked = CreateWhereEqualsLambda<TLinked>("linked", "Id", linkedId);

            Connect().Cypher
               .Match(nodeMatch, linkedMatch)
               .Where(findNode)
               .AndWhere(findLinked)
               .Create(link)
               .WithParam("relParam", linkParams)
               .ExecuteWithoutResults();
            return true;
        }
        

        public List<TLinked> GetNodeLinks<TNode, TLinked>(string nodeId, Linkage linkage)
            where TNode : INode 
            where TLinked:INode
        {
            var query = CreateWhereEqualsLambda<TNode>("Id", nodeId);
            string match = GetMatchLinkageString<TNode, TLinked>(linkage);
                
            return Connect().Cypher
            .OptionalMatch(match)
            .Where(query)
            .Return(linked => linked.As<TLinked>())
            .Results.ToList();

        }

        public List<TNotLinked> GetNodeNotLinks<TNode, TNotLinked>(string nodeId, Linkage linkage)
            where TNode : INode
            where TNotLinked : INode
        {
            var findNode = CreateWhereEqualsLambda<TNode>("Id", nodeId);
            
            string match = GetMatchLabelsString<TNode>("node","linked");
            var noLinkage = "not " +GetMatchAnyLinkageString<TNode,TNotLinked>(linkage);
            return Connect().Cypher
            .OptionalMatch(match)
            .Where(findNode)
            .AndWhere(noLinkage)
            .Return(node => node.As<TNotLinked>())
            .Results.ToList();

        }


        public List<TLinked> GetNodeNewLinks<TNode, TLinked>(string nodeId, Linkage linkage,DateTime dateTime) where TNode : INode
        {
            var findNode = CreateWhereEqualsLambda<TNode>("node", "Id", nodeId);
            var findLinkage = CreateWhereNewerLambda<LinkParams>(dateTime);
            string match = GetMatchLinkageString<TNode, TLinked>(linkage);
            return Connect().Cypher
            .OptionalMatch(match)
            .Where(findNode)
            .AndWhere(findLinkage)
            .Return(linked => linked.As<TLinked>())
            .Results.ToList();

        }
        

        public List<TLinkedBy> GetNodeLinksBy<TNode, TLinkedBy>(string nodeId, Linkage linkage) where TNode : INode
            where TLinkedBy : INode
        {
            var query = CreateWhereEqualsLambda<TNode>("Id", nodeId);
            string match = GetMatchByLinkageString<TNode, TLinkedBy>(linkage);

            return Connect().Cypher
            .OptionalMatch(match)
            .Where(query)
            .Return(linked => linked.As<TLinkedBy>())
            .Results.ToList();

        }
        

        public List<TLinkedByLinkedBy> GetNodeLinkedByLinkedBy<TNode,TLinkedBy,TLinkedByLinkedBy>(string nodeId, Linkage linkage,Linkage linkageBy) 
            where TNode : INode where TLinkedBy:INode where TLinkedByLinkedBy:INode

        {
            var query = CreateWhereEqualsLambda<TNode>("Id", nodeId);
            string match = GetMatchByLinkageByLinkageString<TNode, TLinkedBy, TLinkedByLinkedBy>(linkage, linkageBy);
            return Connect().Cypher
            .OptionalMatch(match)
            .Where(query)
            .Return(linkedbylinkedby => linkedbylinkedby.As<TLinkedByLinkedBy>())
            .Results.ToList();

        }

        public List<MNode> GetNodeLinkedByNotLinkedBy<TNode, TLinkedBy, TLinkedByLinkedBy>(string nodeId, Linkage linkage, Linkage linkageBy)
            where TNode : INode where TLinkedBy : INode where TLinkedByLinkedBy : INode

        {
            var findNode = CreateWhereEqualsLambda<TNode>("Id", nodeId);
            var noLinkage = "r1 is null";
            string match = GetMatchByLinkageByLinkageString<TNode, TLinkedBy, TLinkedByLinkedBy>(linkage, linkageBy);
            return Connect().Cypher
            .OptionalMatch(match)
            .Where(findNode)
            .AndWhere(noLinkage)
            .Return(linkedbylinkedby => linkedbylinkedby.As<MNode>())
            .Results.ToList();

        }

        public List<Tuple<TLinkedBy, TLinkedByLinkedBy>>  GetNodeNewLinkedByLinkedBy<TNode, TLinkedBy, TLinkedByLinkedBy>(string nodeId, Linkage linkage, Linkage linkageBy,DateTime dateTime)
            where TNode : INode where TLinkedBy : INode where TLinkedByLinkedBy : INode
        {
            var findNode = CreateWhereEqualsLambda<TNode>("node", "Id", nodeId);
            var findLinkage = CreateWhereNewerLambda<LinkParams>(dateTime);
            string match = GetMatchByLinkageByLinkageString<TNode, TLinkedBy, TLinkedByLinkedBy>(linkage, linkageBy);

            var results = Connect().Cypher
            .OptionalMatch(match)
            .Where(findNode)
            .AndWhere(findLinkage)
            .Return((linkedby,linkedbylinkedby) => new
               {
                   LinkedBy=linkedby.As<TLinkedBy>(),
                   LinkedByLinkedBy=linkedbylinkedby.As<TLinkedByLinkedBy>()
               }
                   )
             .Results.ToList();
            List<Tuple<TLinkedBy, TLinkedByLinkedBy>> list = new List<Tuple<TLinkedBy, TLinkedByLinkedBy>>();
            results.ForEach(x => list.Add(new Tuple<TLinkedBy, TLinkedByLinkedBy>(x.LinkedBy, x.LinkedByLinkedBy)));
            return list;

        }
       

        public Dictionary<TNode, List<TLinked>> GetNodesLinks<TNode, TLinked>(Linkage linkage)
        where TNode : INode where TLinked : INode
        {
            //var query = CreateWhereEqualsLambda<TNode>("Id", nodeId);
            string match = GetMatchLinkageString<TNode, TLinked>(linkage);
                
            var results = Connect().Cypher
            .OptionalMatch(match)
            //.Where(query)
            .Return((node, linked) => new 
            {
                Node = node.As<TNode>(),
                Links = linked.CollectAs<TLinked>()
            })
            .Results.ToList();
            Dictionary<TNode, List<TLinked>> dict = new Dictionary<TNode, List<TLinked>>();
            results.ForEach(x => dict.Add(x.Node, x.Links.ToList()));
            return dict;

        }
        

        public List<Tuple<TNode, List<TLinkedBy>>> GetNodesLinksBy<TNode, TLinkedBy>(
                                                                                string nodeId,
                                                                                Linkage linkage)
                                                                               
            where TNode : INode where TLinkedBy : INode
        {
            var query = CreateWhereEqualsLambda<TNode>("Id", nodeId);
            string match = string.Format("(node:{0})-[{1}]->(linkedby:{2})",
                                            typeof(TNode).Name, linkage, typeof(TLinkedBy).Name);
            var results = Connect().Cypher
            .OptionalMatch(match)
            .Where(query)
            .Return((node, linked) => new
            {
                Node = node.As<TNode>(),
                Links = linked.CollectAs<TLinkedBy>()
            })
            .Results.ToList();
            List<Tuple<TNode, List<TLinkedBy>>> list = new List<Tuple<TNode, List<TLinkedBy>>>();
            results.ForEach(x => list.Add(new Tuple<TNode, List<TLinkedBy>>(x.Node, x.Links.ToList())));
            return list;

        }
    }
}
