using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace WCF_BiometricoService.Helpers
{
    public static class ExpressionTreeUtil
    {
        public class ReplaceVisitor : ExpressionVisitor
        {
            private readonly Expression from, to;
            public ReplaceVisitor(Expression from, Expression to)
            {
                this.from = from;
                this.to = to;
            }
            public override Expression Visit(Expression node)
            {
                return node == from ? to : base.Visit(node);
            }
        }

        public static Expression Replace(this Expression expression,
            Expression searchEx, Expression replaceEx)
        {
            return new ReplaceVisitor(searchEx, replaceEx).Visit(expression);
        }

        public static Expression BuildBinaryTree<TValue, T>(T valueToCompare, Expression<Func<TValue, T>> expressionToCompareTo, Expression expression, OperadorLogico operador_logico,
             ParameterExpression parametro_ancla, TipoComparacion? tipo_comparacion = null)
        {
            ConstantExpression constant = Expression.Constant(valueToCompare, typeof(T));
            BinaryExpression comparison = null;
            Expression _temp = null;
            if (parametro_ancla != null)
                _temp = expressionToCompareTo.Body.Replace(expressionToCompareTo.Parameters[0], parametro_ancla);
            else
                _temp = expressionToCompareTo.Body;
            switch(operador_logico)
            {
                case OperadorLogico.Equals:
                    comparison = Expression.Equal(_temp, constant);
                    break;
                case OperadorLogico.NotEqual:
                    comparison = Expression.NotEqual(_temp, constant);
                    break;
                case OperadorLogico.GreaterThan:
                    comparison = Expression.GreaterThan(_temp, constant);
                    break;
                case OperadorLogico.GreaterThanOrEqual:
                    comparison = Expression.GreaterThanOrEqual(_temp, constant);
                    break;
                case OperadorLogico.LessThan:
                    comparison = Expression.LessThan(_temp, constant);
                    break;
                case OperadorLogico.LessThanOrEqual:
                    comparison = Expression.LessThanOrEqual(_temp, constant);
                    break;
            }
            
            BinaryExpression newExpression = null;

            if (expression == null)
                newExpression = comparison;
            else if (tipo_comparacion.HasValue && tipo_comparacion.Value == TipoComparacion.AndAlso)
                newExpression = Expression.AndAlso(expression, comparison);
            else if (tipo_comparacion.HasValue && tipo_comparacion.Value == TipoComparacion.OrElse)
                newExpression = Expression.OrElse(expression, comparison);
            return newExpression;
        }
    }

    public enum TipoComparacion
    {
        AndAlso=1,
        OrElse=2
    }

    public enum OperadorLogico
    {
        Equals=1,
        NotEqual=2,
        GreaterThan=3,
        GreaterThanOrEqual=4,
        LessThan=5,
        LessThanOrEqual=6
    }
}