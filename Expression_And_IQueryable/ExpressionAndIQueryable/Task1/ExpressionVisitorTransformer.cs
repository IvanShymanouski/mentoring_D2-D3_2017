using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ExpressionAndIQueryable.Task1
{
    public class ExpressionVisitorTransformer : ExpressionVisitor
    {
        private Dictionary<string, object> MappingDictionary { get; set; }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.Left is ParameterExpression && node.Right is ConstantExpression)
            {
                int value;
                if (int.TryParse(node.Right.ToString(), out value) && value == 1)
                {
                    switch (node.NodeType)
                    {
                        case ExpressionType.Add: return Expression.Increment(node.Left);
                        case ExpressionType.Subtract: return Expression.Decrement(node.Left);
                    }
                }
            }
            return base.VisitBinary(node);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (MappingDictionary != null && MappingDictionary.ContainsKey(node.Name))
            {
                return Expression.Constant(MappingDictionary[node.Name]);
            }

            return base.VisitParameter(node);

        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            return Expression.Lambda(Visit(node.Body), node.Parameters.Where(p => MappingDictionary == null || !MappingDictionary.ContainsKey(p.Name)));
        }

        public Expression Transform(Expression node, Dictionary<string, object> mappingDictionary)
        {
            MappingDictionary = mappingDictionary;

            return Visit(node);
        }
    }
}
