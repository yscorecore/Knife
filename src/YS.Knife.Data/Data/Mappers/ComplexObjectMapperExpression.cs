﻿using System.Linq;
using System.Linq.Expressions;

namespace YS.Knife.Data.Mappers
{
    public class ComplexObjectMapperExpression<TSource, TTarget>: IMapperExpression
         where TSource : class
        where TTarget : class, new()
    {
        private readonly ObjectMapper<TSource, TTarget> objectMapper;
        private readonly LambdaExpression sourceExpression;

        public ComplexObjectMapperExpression(LambdaExpression sourceExpression, ObjectMapper<TSource, TTarget> objectMapper)
        {
            this.objectMapper = objectMapper;
            this.sourceExpression = sourceExpression;
        }

        public LambdaExpression GetLambdaExpression()
        {
            var newObjectExpression = this.objectMapper.BuildExpression();
            var expression = newObjectExpression.ReplaceFirstParam(this.sourceExpression.Body);
            // 需要处理source为null的情况
            var resultExpression = Expression.Condition(
                 Expression.Equal(this.sourceExpression.Body, Expression.Constant(null))
                ,Expression.Constant(null,typeof(TTarget)), expression);

            return Expression.Lambda(resultExpression, this.sourceExpression.Parameters.First());
        }
    }
}
