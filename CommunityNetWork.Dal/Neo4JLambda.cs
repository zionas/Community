using CommunityNetwork.Common.Inerfaces;
using Neo4jClient.Cypher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CommunityNetWork.Dal
{
    public static class Neo4JLambda
    {
        public static Expression<Func<TNode, bool>> CreatePropertyEqualsLambda<TNode>(string tag, string PropertyName, object value) where TNode : INode
        {
            ParameterExpression x = Expression.Parameter(typeof(TNode), tag);
            Expression statusCode = Expression.Property(x, PropertyName);
            Expression comparison = Expression.Equal(statusCode, Expression.Constant(value));
            return Expression.Lambda<Func<TNode, bool>>(comparison, x);
        }

        public static Expression<Func<TNode, TPropertyType>> CreateGetPropertyLambda<TNode, TPropertyType>(string propertyName) where TNode : INode
        {
            ParameterExpression x = Expression.Parameter(typeof(TNode), "node");
            Expression statusCode = Expression.Property(x, propertyName);
            return Expression.Lambda<Func<TNode, TPropertyType>>(statusCode, x);
        }
        public static Expression<Func<ICypherResultItem, TNode>> CreateGetMethodLambda<TNode>(string paramName, string methodName) where TNode : INode
        {
            ParameterExpression x = Expression.Parameter(typeof(ICypherResultItem), paramName);
            Expression statusCode = Expression.Call(x, methodName, new Type[] { typeof(TNode) });
            return Expression.Lambda<Func<ICypherResultItem, TNode>>(statusCode, x);
        }


        public static Expression<Func<TNode, bool>> CreateWhereEqualsLambda<TNode>(string param, string propertyName, object value) where TNode : INode
        {

            ParameterExpression x = Expression.Parameter(typeof(TNode), param);
            Expression statusCode = Expression.Property(x, propertyName);
            Expression comparison = Expression.Equal(statusCode, Expression.Constant(value));
            return Expression.Lambda<Func<TNode, bool>>(comparison, x);
        }

        public static Expression<Func<LinkParams, bool>> CreateWhereNewerLambda<LinkParams>(object value)
        {
            ParameterExpression x = Expression.Parameter(typeof(LinkParams), "r");
            Expression statusCode = Expression.Property(x, "DateTime");
            Expression comparison = Expression.GreaterThanOrEqual(statusCode, Expression.Constant(value));
            return Expression.Lambda<Func<LinkParams, bool>>(comparison, x);
        }
    }
}
