using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace FourTwenty.Core.Specifications
{
    public abstract class BaseSpecification<T> : ISpecification<T>
    {
        public static readonly ISpecification<T> All = new IdentitySpecification<T>();

        public static ISpecification<T> operator |(BaseSpecification<T> left, BaseSpecification<T> right) => left.Or(right);
        public static ISpecification<T> operator &(BaseSpecification<T> left, BaseSpecification<T> right) => left.And(right);
        public static ISpecification<T> operator !(BaseSpecification<T> spec) => new NotSpecification<T>(spec);

        public static implicit operator Expression<Func<T, bool>>(BaseSpecification<T> spec) => spec.ToExpression();

        protected BaseSpecification()
        {
        }


        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();
        public List<string> IncludeStrings { get; } = new List<string>();
        public Expression<Func<T, object>> OrderBy { get; private set; }
        public (string ordering, object[] args)? OrderByDynamic { get; private set; }
        public Expression<Func<T, object>> OrderByDescending { get; private set; }



        public abstract Expression<Func<T, bool>> ToExpression();

        public bool IsSatisfiedBy(T entity)
        {
            Func<T, bool> predicate = ToExpression().Compile();
            return predicate(entity);
        }

        public ISpecification<T> And(ISpecification<T> specification)
        {
            if (this == All)
                return specification;
            if (specification == All)
                return this;

            return new AndSpecification<T>(this, specification);
        }

        public ISpecification<T> Or(ISpecification<T> specification)
        {
            if (this == All || specification == All)
                return All;
            return new OrSpecification<T>(this, specification);
        }

        public int PageNumber { get; private set; }
        public int PageSize { get; private set; }
        public bool IsPagingEnabled { get; private set; } = false;

        protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        protected virtual void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }

        protected virtual void ApplyPaging(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            IsPagingEnabled = true;
        }

        protected virtual void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }

        protected virtual void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
        {
            OrderByDescending = orderByDescendingExpression;
        }

        protected virtual void ApplyOrderBy(string ordering, params object[] args)
        {
            OrderByDynamic = (ordering, args);
        }

        internal virtual void Compose(params ISpecification<T>[] specs)
        {
            var includes = specs.SelectMany(x => x.Includes).Distinct();
            foreach (Expression<Func<T, object>> include in includes)
            {
                AddInclude(include);
            }
            var includeStrings = specs.SelectMany(x => x.IncludeStrings).Distinct();
            foreach (string include in includeStrings)
            {
                AddInclude(include);
            }

            var pagingSpecs = specs.FirstOrDefault(x => x.IsPagingEnabled);
            if (pagingSpecs != null)
            {
                ApplyPaging(pagingSpecs.PageNumber, pagingSpecs.PageSize);
            }

            var orderBy = specs.Select(x => x.OrderBy).FirstOrDefault();
            var orderByDesc = specs.Select(x => x.OrderByDescending).FirstOrDefault();
            var orderByDynamic = specs.Select(x => x.OrderByDynamic).FirstOrDefault();
            if (orderBy != null)
            {
                ApplyOrderBy(OrderBy);
            }
            else if (orderByDesc != null)
            {
                ApplyOrderByDescending(OrderByDescending);
            }
            else if (orderByDynamic != null)
            {
                ApplyOrderBy(orderByDynamic.GetValueOrDefault().ordering);
            }
        }
    }

    internal sealed class IdentitySpecification<T> : BaseSpecification<T>
    {
        public override Expression<Func<T, bool>> ToExpression()
        {
            return x => true;
        }
    }

    internal sealed class AndSpecification<T> : BaseSpecification<T>
    {
        private readonly ISpecification<T> _left;
        private readonly ISpecification<T> _right;

        public AndSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            _right = right;
            _left = left;
            Compose(left, right);
        }

        public override Expression<Func<T, bool>> ToExpression()
        {
            Expression<Func<T, bool>> leftExpression = _left.ToExpression();
            Expression<Func<T, bool>> rightExpression = _right.ToExpression();

            var invokedExpression = Expression.Invoke(rightExpression, leftExpression.Parameters);

            return (Expression<Func<T, bool>>)Expression.Lambda(Expression.AndAlso(leftExpression.Body, invokedExpression), leftExpression.Parameters);
        }
    }

    internal sealed class OrSpecification<T> : BaseSpecification<T>
    {
        private readonly ISpecification<T> _left;
        private readonly ISpecification<T> _right;

        public OrSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            _right = right;
            _left = left;
            Compose(_left, _right);
        }

        public override Expression<Func<T, bool>> ToExpression()
        {
            Expression<Func<T, bool>> leftExpression = _left.ToExpression();
            Expression<Func<T, bool>> rightExpression = _right.ToExpression();

            var invokedExpression = Expression.Invoke(rightExpression, leftExpression.Parameters);

            return (Expression<Func<T, bool>>)Expression.Lambda(Expression.OrElse(leftExpression.Body, invokedExpression), leftExpression.Parameters);
        }
    }

    internal sealed class NotSpecification<T> : BaseSpecification<T>
    {
        private readonly ISpecification<T> _specification;

        public NotSpecification(ISpecification<T> specification)
        {
            _specification = specification;
            Compose(specification);
        }

        public override Expression<Func<T, bool>> ToExpression()
        {
            Expression<Func<T, bool>> expression = _specification.ToExpression();
            UnaryExpression notExpression = Expression.Not(expression.Body);

            return Expression.Lambda<Func<T, bool>>(notExpression, expression.Parameters.Single());
        }
    }
}
