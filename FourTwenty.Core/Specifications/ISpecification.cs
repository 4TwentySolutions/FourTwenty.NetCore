using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FourTwenty.Core.Interfaces
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> ToExpression();
        List<Expression<Func<T, object>>> Includes { get; }
        List<string> IncludeStrings { get; }
        Expression<Func<T, object>> OrderBy { get; }
        Expression<Func<T, object>> OrderByDescending { get; }
        (string ordering, object[] args)? OrderByDynamic { get; }
        bool IsSatisfiedBy(T entity);
        ISpecification<T> And(ISpecification<T> specification);
        ISpecification<T> Or(ISpecification<T> specification);
        int PageNumber { get; }
        int PageSize { get; }
        bool IsPagingEnabled { get; }
    }
}
