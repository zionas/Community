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
            TimeStamp = DateTime.UtcNow;
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
        const string MatchDefinedLabelFormat = "(node:{0})";
        const string CreateFormat = "(node:{0}{{newNode}})";
        const string MatchDeleteLabelFormat = "(unlinkednode:{0}),(linkednode:{0})-[r]-()";
        const string MatchDefinedLinkageFormat = "(linkedBy:{0})<-[r:{1}]-(linker:{2})";
        
        const string MatchAnyDefinedLinkageFormat = "(node1:{0})-[r:{1}]-(node2:{2})";

        const string MatchByDefinedLinkageByLinkageFormat = "(linker:{0})-[r1:{1}]->(linkedby:{2})-[r2:{3}]->(linkedbylinkedby:{4})";

        const string MatchByUndefinedLinkageByLinkageFormat = "(linker:{0})-[:{1}]->(linkedby:{2})-[:{3}]->(linkedbylinkedby:{4})";

        const string MatchUndefinedLinkageFormat = "(linkedBy:{0})<-[:{1}]-(linker:{2})";
        const string MatchAnyUndefinedLinkageFormat = "(node1)-[:{0}]-(node2)";
        
        const string ByLinkageFormat = "(linker)-[:{0}]->(linkedBy)";

        const string CreateAndLinkFormat = "(linker:{0})-[:{1}]->(linkedBy:{2}{{newNode}})";
        static readonly string LinkageWithParamsFormat = "(linkedBy)<-[:{0} {{relParam}}]-(linker)";
        const string CreateAndLinkWithParamsFormat = "(linker:{0})-[:{0}{{relParam}}]->(linkedBy:{1}{{newNode}})";
                
        static readonly string LinkageFormat = "(linker)-[:{0}]->(linkedBy)";
        
        static readonly string ByLinkageWithParamsFormat = "(linker)-[:{0} {{relParam}}]->(linkedBy)";

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

        static Expression<Func<TNode, bool>> CreatePropertyEqualsLambda<TNode>(string tag,string PropertyName, object value) where TNode : INode
        {
            ParameterExpression x = Expression.Parameter(typeof(TNode), tag);
            Expression statusCode = Expression.Property(x, PropertyName);
            Expression comparison = Expression.Equal(statusCode, Expression.Constant(value));
            return Expression.Lambda<Func<TNode, bool>>(comparison, x);
        }

        static Expression<Func<TNode, TPropertyType>> CreateGetPropertyLambda<TNode,TPropertyType>(string propertyName) where TNode : INode
        {
            ParameterExpression x = Expression.Parameter(typeof(TNode), "node");
            Expression statusCode = Expression.Property(x, propertyName);
            return Expression.Lambda<Func<TNode, TPropertyType>>(statusCode, x);
        }
        static Expression<Func<ICypherResultItem,TNode>> CreateGetMethodLambda<TNode>(string paramName,string methodName) where TNode : INode
        {
            ParameterExpression x = Expression.Parameter(typeof(ICypherResultItem), paramName);
            Expression statusCode = Expression.Call(x, methodName, new Type[] { typeof(TNode) });
            return Expression.Lambda<Func<ICypherResultItem,TNode>>(statusCode,x);
        }


        static Expression<Func<TNode, bool>> CreateWhereEqualsLambda<TNode>(string param, string propertyName, object value) where TNode : INode
        {

            ParameterExpression x = Expression.Parameter(typeof(TNode), param);
            Expression statusCode = Expression.Property(x, propertyName);
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
        
        
        static ICypherFluentQuery WhereQuery<TNode>(ICypherFluentQuery query, List<Tuple<string, object>> valuesToFind) where TNode : INode
        {
            valuesToFind.ForEach(keyValue =>
            {
                var lambda = CreatePropertyEqualsLambda<TNode>("node",keyValue.Item1, keyValue.Item2);
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
            var query = CreatePropertyEqualsLambda<TNode>("node","Id", id);
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

            string format = MatchDefinedLabelFormat ;
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
            return string.Format(MatchDefinedLinkageFormat, TypeName<TNode>(),linkage,TypeName<TLinked>());
        }
        

        static string GetMatchByLinkageString<TNode, TLinkedBy>(Linkage linkage)
        {
            return string.Format(MatchDefinedLinkageFormat, TypeName<TNode>(),linkage, TypeName<TLinkedBy>());
        }
        

        static string GetMatchByLinkageByLinkageString<TNode, TLinkedBy,TLinkedByLinkedBy>(Linkage linkage,Linkage linkageBy)
        {
            return string.Format(MatchByDefinedLinkageByLinkageFormat, TypeName<TNode>(),linkage, TypeName<TLinkedBy>(), linkageBy,TypeName<TLinkedByLinkedBy>());
        }
        


        static string GetCreateString<TNode>()
        {
            return string.Format(CreateFormat, TypeName<TNode>());
        }
        private static string GetLinkageString(Linkage linkage)
        {
            return string.Format(LinkageFormat, linkage);
        }
        static string GetMatchDeleteLabelString<TNode>()
        {
            return string.Format(MatchDeleteLabelFormat,TypeName<TNode>());
        }

        private static string GetMatchOneDefinedLinkageString<TNode, TLinked>(Linkage linkage)
        {
            return string.Format(MatchDefinedLinkageFormat, TypeName<TNode>(), linkage, TypeName<TLinked>());
        }


        private static string GetMatchAnyDefinedLinkageString<TNode, TLinked>(Linkage linkage)
        {
            return string.Format(MatchAnyDefinedLinkageFormat, TypeName<TNode>(), linkage, TypeName<TLinked>());
        }
        private static string GetMatchAnyLinkageString<TNode, TLinked>(Linkage linkage)
        {
            return string.Format(MatchAnyDefinedLinkageFormat, TypeName<TNode>(), linkage, TypeName<TLinked>());
        }
        private static string GetMatchAnyLinkageString(Linkage linkage)
        {
            return string.Format(MatchAnyUndefinedLinkageFormat,  linkage );
        }

       
        private static string GetMatchDefinedLinkageString<TNode, TLinked>(Linkage linkage)
        {
            return string.Format(MatchDefinedLinkageFormat, TypeName<TNode>(), linkage, TypeName<TLinked>());
        }

        private static string GetLinkageWithParamsString(Linkage linkage)
        {
            return string.Format(LinkageWithParamsFormat, linkage);
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
            query.CreateUnique(string.Format(LinkageFormat, linkage)).ExecuteWithoutResults();
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
            var query = CreatePropertyEqualsLambda<TNode>("linkednode","Id", id);
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

        }


        public bool Delete<TNode>() where TNode : INode
        {
            string typeName = TypeName<TNode>();
            string match = "(unlinkednode:" + TypeName<TNode>() + ")";//GetMatchDeleteLabelString<TNode>();

            try
            {
                
                Connect().Cypher.Match(match)
                    .DetachDelete("unlinkednode")
                    .ExecuteWithoutResults();
            }
            catch (Exception e)
            {
                throw e;
            }

            return true;

        }
        public bool IsLinker<TLinkedBy,TLinker>(string nodeId,string linkedId,Linkage linkage)
            where TLinkedBy:INode where TLinker:INode
        {
            var linkMatch = GetMatchLinkageString<TLinkedBy, TLinker>(linkage);
            var findLinkedBy = CreateWhereEqualsLambda<TLinkedBy>("linkedBy", "Id", nodeId);
            var findLinker = CreateWhereEqualsLambda<TLinker>("linker", "Id", linkedId);

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
            var linkByMatch = GetMatchByLinkageString<TLinkedBy, TLinkedBy>(linkage);
            var findNode = CreateWhereEqualsLambda<TLinkedBy>("linker", "Id", linkedById);
            var findLinked = CreateWhereEqualsLambda<TLinkedBy>("linkedby", "Id", linkerId);

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
            var link = GetLinkageString(linkage);
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
            var link = GetLinkageWithParamsString(linkage);
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
        

        public bool Link<TNode, TLinked>(string linkedById, string linkerId, Linkage linkage) where TNode : INode where TLinked : INode
        {
            var nodeMatch = GetMatchLabelString<TNode>("linkedBy");
            var linkedMatch = GetMatchLabelString<TLinked>("linker");
            var link = GetLinkageString( linkage);
            var findNode = CreateWhereEqualsLambda<TNode>("linkedBy", "Id",linkedById);
            var findLinked = CreateWhereEqualsLambda<TLinked>("linker", "Id",linkerId);

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
        

        public bool LinkWithParams<TNode, TLinked>(
            string nodeId,
            string linkedId,
            Linkage linkage,
            LinkParams linkParams
            )
            where TNode : INode where TLinked : INode
        {
            var nodeMatch = GetMatchLabelString<TNode>("node");
            var linkedMatch = GetMatchLabelString<TLinked>("linked");
            var link = string.Format(LinkageWithParamsFormat, linkage);
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

        public List<LinkedNode> GetNodeLinkersWithData<TLinkedBy, TLinker>(
            string linkedById,
            Linkage linkage
            )
            where TLinkedBy : INode
            where TLinker : INode
        {
            string match = GetMatchByLinkageString<TLinkedBy, TLinker>(linkage);

            return Connect().Cypher
                .Match(match)
                .Where((MNode n) => n.Id == linkedById)
                .Return(CreateLinkageResultLambda())
                 .Results.ToList();
        }

        public List<TLinker> GetNodeLinkers<TLinkedBy, TLinker>(
            string nodeId, 
            Linkage linkage
            )
            where TLinkedBy : INode
            where TLinker : INode
        {
            var query = CreatePropertyEqualsLambda<TLinkedBy>("linkedBy","Id", nodeId);
            string match = GetMatchLinkageString<TLinkedBy, TLinker>(linkage);

            return Connect().Cypher
            .Match(match)
            .Where(query)
            .Return(linker => linker.As<TLinker>())
            .Results.ToList();

        }

       public ICypherFluentQuery<TLinker> GetNodeLinkersResults<TLinkedBy, TLinker>(
            string nodeId,
            Linkage linkage
            )
            where TLinkedBy : INode
            where TLinker : INode
        {
            var query = CreatePropertyEqualsLambda<TLinkedBy>("linkedBy", "Id", nodeId);
            string match = GetMatchLinkageString<TLinkedBy, TLinker>(linkage);

            return Connect().Cypher
            .OptionalMatch(match)
            .Where(query)
            .Return(linker => linker.As<TLinker>());
            
        }

    public List<TLinkedBy> GetNodeLinkedBy<TLinker,TLinkedBy>(
            string linkerId,
            Linkage linkage
            )
            where TLinkedBy : INode
            where TLinker : INode
        {
            var query = CreateWhereEqualsLambda<TLinker>("linker","Id", linkerId);
            string match = GetMatchLinkageString<TLinkedBy, TLinker>(linkage);

            return Connect().Cypher
            .OptionalMatch(match)
            .Where(query)
            .Return(linkedBy => linkedBy.As<TLinkedBy>())
            .OrderBy("linkedBy.CreateTime DESC")
            .Results.ToList();
        }
        
        public List<TNotLinked> GetNodeNotLinks<TNode, TNotLinked>(
            string nodeId,
            Linkage linkage
            )
            where TNode : INode
            where TNotLinked : INode
        {
            var findNode = CreatePropertyEqualsLambda<TNode>("node1","Id", nodeId);
            
            string match = GetMatchLabelsString<TNode>("node1","node2");
            var noLinkage = "not " +GetMatchAnyLinkageString(linkage);
            return Connect().Cypher
            .OptionalMatch(match)
            .Where(findNode)
            .AndWhere(noLinkage)
            .Return(node2=> node2.As<TNotLinked>())
            .Results.ToList();
        }

        public List<TNotLinked> GetNodeNotLinks<TNode, TNotLinked>(
            ICypherFluentQuery results,
            string with,
            string nodeId,
            Linkage linkage
            )
            where TNode : INode
            where TNotLinked : INode
        {
            var findNode = CreatePropertyEqualsLambda<TNode>("node1", "Id", nodeId);

            string match = GetMatchLabelsString<TNode>("node1", "node2");
            var noLinkage = "not " + GetMatchAnyLinkageString(linkage);
            var returnLambda = CreateGetMethodLambda<TNotLinked>("node2", "As");
            return results
            .With(with)
            .Where(noLinkage)
            .Return(returnLambda)
            .Results.ToList();

        }


        public ICypherFluentQuery GetNodeLinkedByResults<TLinker, TLinkedBy>(string linkerId, Linkage linkage
            )
            where TLinkedBy : INode
            where TLinker : INode
        {
            string inputTag = "linker";
            string outputTag = "linkedby";
            var returnLambda = CreateGetMethodLambda<TLinkedBy>(outputTag, "As");
            var query = CreateWhereEqualsLambda<TLinker>(inputTag, "Id", linkerId);
            string match = GetMatchLinkageString<TLinkedBy, TLinker>(linkage);

            return Connect().Cypher
            .Match(match)
            .Where(query);            
            //.Return(returnLambda)
            //.OrderBy(outputTag+".CreateTime DESC");
           

        }

        public ICypherFluentQuery<TLinkedBy> GetNodesLinkedByResults<TLinker, TLinkedBy>(
            ICypherFluentQuery<TLinker> linkersResults,
            string linkerId, 
            Linkage linkage
            )
            where TLinkedBy : INode
            where TLinker : INode
        {
            string inputTag = "linker";
            string outputTag = "linkedby";
            
            string match = GetMatchLinkageString<TLinkedBy, TLinker>(linkage);

            return linkersResults
            .With(inputTag)
            .OptionalMatch(match)
            .Return(CreateGetMethodLambda<TLinkedBy>(outputTag, "As"))
            .OrderBy(outputTag + ".CreateTime DESC");


        }
        public List<TLinked> GetNodeNewLinks<TNode, TLinked>(
            string nodeId,
            Linkage linkage,
            DateTime dateTime
            ) where TNode : INode
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
        

        public List<TLinkedBy> GetNodeLinksBy<TLinker, TLinkedBy>(
            string nodeId,
            Linkage linkage) 
            where TLinker : INode
            where TLinkedBy : INode
        {
            var query = CreatePropertyEqualsLambda<TLinker>("linker","Id", nodeId);
            string match = GetMatchByLinkageString<TLinker, TLinkedBy>(linkage);

            return Connect().Cypher
            .OptionalMatch(match)
            .Where(query)
            .Return(linkedBy => linkedBy.As<TLinkedBy>())
            .Results.ToList();

        }
        

        public List<TLinkedByLinkedBy> GetNodeLinkedByLinkedBy <TLinker,TLinkedBy,TLinkedByLinkedBy>(
            string nodeId,
            Linkage linkage,
            Linkage linkageBy) 
            where TLinker : INode where TLinkedBy:INode where TLinkedByLinkedBy:INode

        {
            var query = CreatePropertyEqualsLambda<TLinker>("linker","Id", nodeId);
            
            string match = GetMatchByLinkageByLinkageString<TLinker, TLinkedBy, TLinkedByLinkedBy>(linkage, linkageBy);
            return Connect().Cypher
            .OptionalMatch(match)
            .Where(query)
            
            .Return(linkedbylinkedby => linkedbylinkedby.As<TLinkedByLinkedBy>())
            .OrderBy("linkedbylinkedby.CreateTime DESC")
            .Results.ToList();

        }

        public List<MNode> GetNodeLinkedByNotLinkedBy<TLinker, TLinkedBy, TLinkedByLinkedBy>(
            string nodeId,
            Linkage linkage,
            Linkage linkageBy)
            where TLinker : INode where TLinkedBy : INode where TLinkedByLinkedBy : INode

        {
            var findNode = CreatePropertyEqualsLambda<TLinker>("linker","Id", nodeId);
            var noLinkage = "r1 is null";
            string match = GetMatchByLinkageByLinkageString<TLinker, TLinkedBy, TLinkedByLinkedBy>(linkage, linkageBy);
            return Connect().Cypher
            .OptionalMatch(match)
            .Where(findNode)
            .AndWhere(noLinkage)
            .Return(linkedbylinkedby => linkedbylinkedby.As<MNode>())
            .Results.ToList();

        }

        public List<Tuple<TLinkedBy, TLinkedByLinkedBy>>  GetNodeNewLinkedByLinkedBy<TNode, TLinkedBy, TLinkedByLinkedBy>(
            string nodeId, 
            Linkage linkage,
            Linkage linkageBy,
            DateTime dateTime)
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
       

        public Dictionary<TLinkedBy, List<TLinker>> GetNodesLinks<TLinkedBy, TLinker>(Linkage linkage)
        where TLinkedBy : INode where TLinker : INode
        {
            
            string match = GetMatchLinkageString<TLinkedBy, TLinker>(linkage);
                
            var results = Connect().Cypher
            .OptionalMatch(match)
            //.Where(query)
            .Return((linkedBy, linker) => new 
            {
                Node = linkedBy.As<TLinkedBy>(),
                Links = linker.CollectAs<TLinker>()
            })
            .Results.ToList();
            Dictionary<TLinkedBy, List<TLinker>> dict = new Dictionary<TLinkedBy, List<TLinker>>();
            results.ForEach(x => dict.Add(x.Node, x.Links.ToList()));
            return dict;

        }
        

        public List<Tuple<TLinker, List<TLinkedBy>>> GetNodesLinksBy<TLinker, TLinkedBy>(                                                                            string nodeId,
          Linkage linkage)
                                                                               
            where TLinker : INode where TLinkedBy : INode
        {
            var query = CreatePropertyEqualsLambda<TLinker>("linker","Id", nodeId);
            string match = string.Format("(node:{0})-[{1}]->(linkedby:{2})",
                                            typeof(TLinker).Name, linkage, typeof(TLinkedBy).Name);
            var results = Connect().Cypher
            .OptionalMatch(match)
            .Where(query)
            .Return((node, linked) => new
            {
                Node = node.As<TLinker>(),
                Links = linked.CollectAs<TLinkedBy>()
            })
            .Results.ToList();
            List<Tuple<TLinker, List<TLinkedBy>>> list = new List<Tuple<TLinker, List<TLinkedBy>>>();
            results.ForEach(x => list.Add(new Tuple<TLinker, List<TLinkedBy>>(x.Node, x.Links.ToList())));
            return list;

        }
    }
}
