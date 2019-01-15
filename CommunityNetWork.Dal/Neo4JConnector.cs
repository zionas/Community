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

        const string CreateAndLinkFormat = "(linker:{0})-[:{1}]->(linkedBy:{2}{{newNode}})";

        const string MatchDeleteLabelFormat = "(unlinkednode:{0}),(linkednode:{0})-[r]-()";

        const string MatchDetachFormat = "(node:{0})";

        const string MatchAnyDefinedLinkageFormat = "(node1:{0})-[r:{1}]-(node2:{2})";
        const string MatchAnyUndefinedLinkageFormat = "(node1)-[:{0}]-(node2)";

        const string MatchFullDefinedLinkageFormat = "(linkedBy:{0})<-[r:{1}]-(linker:{2})";

        const string MatchLinkedByUndefinedLinkageFormat = "(linkedBy:{0})<-[:{1}]-(:{2})";

        const string MatchFullUndefinedLinkageFormat = "(linkedBy:{0})<-[:{1}]-(linker:{2})";

        const string MatchLinkedByLinkedByUndefinedLinkageFormat =  "(linkedByLinkedBy:{0})<-[:{1}]-(:{2})";
                 
        const string MatchFullDefinedLinkageByLinkageFormat = "(linker:{0})-[r1:{1}]->(linkedBy:{2})-[r2:{3}]->(linkedByLinkedBy:{4})";

        const string MatchFullUndefinedLinkageByLinkageFormat = "(linker:{0})-[:{1}]->(linkedBy:{2})-[:{3}]->(linkedByLinkedBy:{4})";
             
        
                       
        const string LinkageFormat =   "(linker)-[:{0}]->(linkedBy)";
        
        static readonly string ByLinkageWithParamsFormat = "(linker)-[:{0} {{relParam}}]->(linkedBy)";
        static readonly string LinkageWithParamsFormat = "(linkedBy)<-[:{0} {{relParam}}]-(linker)";

        const string CreateAndLinkWithParamsFormat = "(linker:{0})-[:{0}{{relParam}}]->(linkedBy:{1}{{newNode}})";


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

        public string GetTypeName(string id) 
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
        

        static string GetMatchDefinedLabelString<TNode>()
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


        static string GetMatchLinkedByUndefinedLinkageString<TLinkedBy, TLinker>(Linkage linkage)
        {
            return string.Format(MatchLinkedByUndefinedLinkageFormat, TypeName<TLinkedBy>(), linkage, TypeName<TLinker>());
        }


        static string GetMatchLinkedByLinkedByUndefinedLinkageString<TLinkedByLinkedBy, TLinkedBy>(Linkage linkage)
        {
            return string.Format(MatchLinkedByLinkedByUndefinedLinkageFormat, TypeName<TLinkedByLinkedBy>(), linkage, TypeName<TLinkedBy>());
        }


        static string GetMatchFullDefinedLinkageString<TLinkedBy,TLinker>(Linkage linkage)
        {
            return string.Format(MatchFullDefinedLinkageFormat, TypeName<TLinkedBy>(),linkage,TypeName<TLinker>());
        }
       
        static string GetMatchByLinkageByLinkageString<TNode, TLinkedBy,TLinkedByLinkedBy>(Linkage linkage,Linkage linkageBy)
        {
            return string.Format(MatchFullDefinedLinkageByLinkageFormat, TypeName<TNode>(),linkage, TypeName<TLinkedBy>(), linkageBy,TypeName<TLinkedByLinkedBy>());
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


        static string GetMatchDetachString<TNode>()
        {
            return string.Format(MatchDetachFormat, TypeName<TNode>());
        }

        private static string GetMatchOneDefinedLinkageString<TNode, TLinked>(Linkage linkage)
        {
            return string.Format(MatchFullDefinedLinkageFormat, TypeName<TNode>(), linkage, TypeName<TLinked>());
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
            return string.Format(MatchFullDefinedLinkageFormat, TypeName<TNode>(), linkage, TypeName<TLinked>());
        }

        private static string GetLinkageWithParamsString(Linkage linkage)
        {
            return string.Format(LinkageWithParamsFormat, linkage);

        }

        private static string GetCreateAndLinkString<TLinked,TNew>(Linkage linkage)
        {
            return string.Format(CreateAndLinkFormat, TypeName < TLinked >(), linkage, TypeName<TNew>());
        }

        private static string GetCreateAndLinkWithParamsString<TLinked, TNew>(Linkage linkage)
        {
            return string.Format(CreateAndLinkWithParamsFormat, TypeName<TLinked>(), linkage, TypeName<TNew>());
        }
                

        private static void CreateByLinkageWithParam(ICypherFluentQuery query, Linkage linkage, LinkParams linkParams)
        {
           
            query.CreateUnique(string.Format(ByLinkageWithParamsFormat, linkage))
                    .WithParam("relParam", linkParams)
                    .ExecuteWithoutResults();
        }
        

        private static void CreateLinkage(ICypherFluentQuery query, Linkage linkage)
        {
            query.CreateUnique(string.Format(LinkageFormat, linkage)).ExecuteWithoutResults();
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
                CreateLinkage(query, linkage);
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
            string match = GetMatchDetachString<TNode>();
                          
                Connect().Cypher.Match(match)
                    .DetachDelete("node")
                    .ExecuteWithoutResults();
            

            return true;

        }

        public bool DeleteAll() 
        {
            Connect().Cypher.Match("(node)")
                .With("(node) limit 1000")
                .DetachDelete("node")
                .ExecuteWithoutResults();

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
        

        public TNew CreateAndLinkWithParams<TNew,TLinked>(string linkerId, TNew newNode, Linkage linkage, LinkParams linkParams) 
            where TNew: INode
            where TLinked:INode
        {
           
            var linkerMatch = GetMatchLabelString<TLinked>("linker");
            var link = GetLinkageWithParamsString(linkage);
            var findLinker = CreateWhereEqualsLambda<TLinked>("linker", "Id", linkerId);
            string create = GetCreateString<TNew>();

            return Connect().Cypher
                .Create(create)
                .WithParam("newNode", newNode)
                .With("node")
                .Match(linkerMatch)
                .Where(findLinker)
                .Merge(link)
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
               .Merge(link)
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
        

        public bool LinkWithParams<TNode, TLinked>(
            string linkedById,
            string linkerId,
            Linkage linkage,
            LinkParams linkParams
            )
            where TNode : INode where TLinked : INode
        {
            var nodeMatch = GetMatchLabelString<TNode>("linkedBy");
            var linkedMatch = GetMatchLabelString<TLinked>("linker");
            var link = string.Format(LinkageWithParamsFormat, linkage);
            
            var findLinkedBy = CreateWhereEqualsLambda<TNode>("linkedBy", "Id", linkedById);
            var findLinker = CreateWhereEqualsLambda<TLinked>("linker", "Id", linkerId);

            Connect().Cypher
               .Match(nodeMatch, linkedMatch)
               .Where(findLinkedBy)
               .AndWhere(findLinker)
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
            string match = GetMatchFullDefinedLinkageString<TLinkedBy, TLinker>(linkage);

            return Connect().Cypher
                .Match(match)
                .Where((MNode n) => n.Id == linkedById)
                .Return(CreateLinkageResultLambda())
                 .Results.ToList();
        }

        public List<TLinker> GetNodeLinkers<TLinkedBy, TLinker>(
            string linkedById, 
            Linkage linkage
            )
            where TLinkedBy : INode
            where TLinker : INode
        {
            var query = CreatePropertyEqualsLambda<TLinkedBy>("linkedBy","Id", linkedById);
            string match = GetMatchFullDefinedLinkageString<TLinkedBy, TLinker>(linkage);

            return Connect().Cypher
            .Match(match)
            .Where(query)
            .Return(linker => linker.As<TLinker>())
            .Results.ToList();

        }

       public ICypherFluentQuery GetNodeLinkersResults<TLinkedBy, TLinker>(
            string linkedById,
            Linkage linkage
            )
            where TLinkedBy : INode
            where TLinker : INode
        {
            var query = CreatePropertyEqualsLambda<TLinkedBy>("linkedBy", "Id", linkedById);
            string match = GetMatchFullDefinedLinkageString<TLinkedBy, TLinker>(linkage);

            return Connect().Cypher
            .Match(match)
            .Where(query);
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

        public ICypherFluentQuery GetNodeNotLinksResults<TNode, TNotLinked>(
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
            .Where(noLinkage);
            

        }

        public ICypherFluentQuery GetNodeLinkedByResults<TLinker, TLinkedBy>(string linkerId, Linkage linkage
            )
            where TLinkedBy : INode
            where TLinker : INode
        {
            string inputTag = "linker";
            string outputTag = "linkedBy";
            var returnLambda = CreateGetMethodLambda<TLinkedBy>(outputTag, "As");
            var query = CreateWhereEqualsLambda<TLinker>(inputTag, "Id", linkerId);
            string match = GetMatchFullDefinedLinkageString<TLinkedBy, TLinker>(linkage);

            return Connect().Cypher
            .Match(match)
            .Where(query);            
            
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
            string outputTag = "linkedBy";
            
            string match = GetMatchFullDefinedLinkageString<TLinkedBy, TLinker>(linkage);

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
            string match = GetMatchFullDefinedLinkageString<TNode, TLinked>(linkage);
            return Connect().Cypher
            .OptionalMatch(match)
            .Where(findNode)
            .AndWhere(findLinkage)
            .Return(linked => linked.As<TLinked>())
            .Results.ToList();

        }

        public List<TLinkedBy> GetNodeLinkedBy<TLinker, TLinkedBy>(
                    string linkerId,
                    Linkage linkage
                    )
                    where TLinkedBy : INode
                    where TLinker : INode
        {
            var query = CreateWhereEqualsLambda<TLinker>("linker", "Id", linkerId);
            string match = GetMatchFullDefinedLinkageString<TLinkedBy, TLinker>(linkage);

            return Connect().Cypher
            .OptionalMatch(match)
            .Where(query)
            .Return(linkedBy => linkedBy.As<TLinkedBy>())
            .OrderBy("linkedBy.CreateTime DESC")
            .Results.ToList();
        }
        
        

        public List<TLinkedByLinkedBy> GetNodeLinkedByLinkedBy <TLinker,TLinkedBy,TLinkedByLinkedBy>(
            string linkerId,
            Linkage linkage,
            Linkage linkageBy) 
            where TLinker : INode where TLinkedBy:INode where TLinkedByLinkedBy:INode

        {
            var query = CreatePropertyEqualsLambda<TLinker>("linker","Id", linkerId);
            
            string match = GetMatchByLinkageByLinkageString<TLinker, TLinkedBy, TLinkedByLinkedBy>(linkage, linkageBy);
            return Connect().Cypher
            .OptionalMatch(match)
            .Where(query)
            
            .Return(linkedByLinkedBy => linkedByLinkedBy.As<TLinkedByLinkedBy>())
            .OrderBy("linkedByLinkedBy.CreateTime DESC")
            .Results.ToList();

        }

        public ICypherFluentQuery GetNodeLinkedByLinkedByResults <TLinker, TLinkedBy, TLinkedByLinkedBy>(
            string linkerId,
            Linkage linkage,
            Linkage linkageBy)
            where TLinker : INode where TLinkedBy : INode where TLinkedByLinkedBy : INode

        {
            var query = CreatePropertyEqualsLambda<TLinker>("linker", "Id", linkerId);

            string match = GetMatchByLinkageByLinkageString<TLinker, TLinkedBy, TLinkedByLinkedBy>(linkage, linkageBy);
            return Connect().Cypher
            .OptionalMatch(match)
            .Where(query);


        }

        public List<MNode> GetNodeLinkedByNotLinkedBy<TLinker, TLinkedBy, TLinkedByLinkedBy>(
            string linkerId,
            Linkage linkage,
            Linkage linkageBy)
            where TLinker : INode where TLinkedBy : INode where TLinkedByLinkedBy : INode

        {
            var findNode = CreatePropertyEqualsLambda<TLinker>("linker","Id", linkerId);
            var noLinkage = "r1 is null";
            string match = GetMatchByLinkageByLinkageString<TLinker, TLinkedBy, TLinkedByLinkedBy>(linkage, linkageBy);
            return Connect().Cypher
            .OptionalMatch(match)
            .Where(findNode)
            .AndWhere(noLinkage)
            .Return(linkedByLinkedBy => linkedByLinkedBy.As<MNode>())
            .Results.ToList();

        }

        
        public Dictionary<TLinkedBy, List<TLinker>> GetNodesLinks<TLinkedBy, TLinker>(Linkage linkage)
        where TLinkedBy : INode where TLinker : INode
        {
            
            string match = GetMatchFullDefinedLinkageString<TLinkedBy, TLinker>(linkage);
                
            var results = Connect().Cypher
            .OptionalMatch(match)
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
        

        public List<Tuple<TLinker, List<TLinkedBy>>> GetNodesLinksBy<TLinker, TLinkedBy>(                                                                            string linkerId,
          Linkage linkage)
                                                                               
            where TLinker : INode where TLinkedBy : INode
        {
            var query = CreatePropertyEqualsLambda<TLinker>("linker","Id", linkerId);
            string match = string.Format("(linker:{0})-[{1}]->(linkedBy:{2})",
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

        public List<TLinkedBy> GetNodesWithLinkersCount<TLinkedBy,TLinker>(ICypherFluentQuery results, Linkage linkage, string tag)
            where TLinkedBy:INode
            where TLinker: INode
        {
            string countPropertyName = linkage.ToString() + "s";
            string match = tag == "linkedBy" ? GetMatchLinkedByUndefinedLinkageString<TLinkedBy, TLinker>(linkage)
                : GetMatchLinkedByLinkedByUndefinedLinkageString<TLinkedBy, TLinker>(linkage);

            var with = string.Format("{0}, size("+ match+") as {3}", tag, linkage.ToString(), TypeName<TLinkedBy>(), countPropertyName);
            
            var setCount = string.Format("{0}.{1}={1}", tag, countPropertyName);
            var returnLambda = CreateGetMethodLambda<TLinkedBy>(tag, "As");
            var orderBy = tag + ".CreateTime DESC";
            return results
            .With(with)
            .Set(setCount)
            .Return(returnLambda)
            .OrderBy(orderBy)
            .Results.ToList();
        }

        public ICypherFluentQuery GetNodesWithLinkersCountResults<TLinkedBy, TLinker>(ICypherFluentQuery results, Linkage linkage, string tag)
            where TLinkedBy : INode
            where TLinker:INode
        {
            string countPropertyName = linkage.ToString() + "s";
            var with = string.Format("{0}, size(" + GetMatchLinkedByUndefinedLinkageString<TLinkedBy, TLinker>(linkage) + ") as {3}", tag, linkage.ToString(), TypeName<TLinkedBy>(), countPropertyName);


            var setCount = string.Format("{0}.{1}={1}", tag, countPropertyName);
            var returnLambda = CreateGetMethodLambda<TLinkedBy>(tag, "As");
            var orderBy = tag + ".CreateTime DESC";
            return results
            .With(with)
            .Set(setCount);
            
        }
        public List<TResult> ReturnResults<TResult>(ICypherFluentQuery results, string tag) where TResult : INode
        {
            var returnLambda = CreateGetMethodLambda<TResult>(tag, "As");
            var orderBy = tag + ".CreateTime DESC";
            return results
                .Return(returnLambda)
            .OrderBy(orderBy)
            .Results.ToList();
        }


            public List<TNode> GetNodesWithLinksCollections<TNode>(ICypherFluentQuery results, Linkage linkage, string tag)
            where TNode : INode
        {
            string linksSuffix = tag.EndsWith("er") ? "ers" : "s";
            string collectionPropertyName = linkage.ToString() +linksSuffix;
            var with = string.Format("{0}, collect(({0}) -[:{1}]->(: {2})) as {3}", tag, linkage.ToString(), TypeName<TNode>(), collectionPropertyName);

            var setCount = string.Format("{0}.{1}={1}", tag, collectionPropertyName);
            return results
            .With(with)
            .Set(setCount)
            .Return(CreateGetMethodLambda<TNode>(tag, "As"))
            .Results.ToList();
        }

        ////////////////////////////////////////////////////////
        ///to rewrite
        public List<Tuple<TLinkedBy, TLinkedByLinkedBy>> GetNodeNewLinkedByLinkedBy<TLinker, TLinkedBy, TLinkedByLinkedBy>(
            string linkerId,
            Linkage linkage,
            Linkage linkageBy,
            DateTime dateTime)
            where TLinker : INode where TLinkedBy : INode where TLinkedByLinkedBy : INode
        {
            var findNode = CreateWhereEqualsLambda<TLinker>("linker", "Id", linkerId);
            var findLinkage = CreateWhereNewerLambda<LinkParams>(dateTime);
            string match = GetMatchByLinkageByLinkageString<TLinker, TLinkedBy, TLinkedByLinkedBy>(linkage, linkageBy);

            var results = Connect().Cypher
            .OptionalMatch(match)
            .Where(findNode)
            .AndWhere(findLinkage)
            .Return((linkedBy, linkedByLinkedBy) => new
            {
                LinkedBy = linkedBy.As<TLinkedBy>(),
                LinkedByLinkedBy = linkedByLinkedBy.As<TLinkedByLinkedBy>()
            }
                   )
             .Results.ToList();
            List<Tuple<TLinkedBy, TLinkedByLinkedBy>> list = new List<Tuple<TLinkedBy, TLinkedByLinkedBy>>();
            results.ForEach(x => list.Add(new Tuple<TLinkedBy, TLinkedByLinkedBy>(x.LinkedBy, x.LinkedByLinkedBy)));
            return list;

        }


    }
}
