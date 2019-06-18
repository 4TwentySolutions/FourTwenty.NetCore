using System.Linq;
using System.Linq.Dynamic.Core;
using FourTwenty.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FourTwenty.Core.Specifications
{
    public class SpecificationEvaluator<T> where T : class
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
        {
            var query = inputQuery;

            // modify the IQueryable using the specification's criteria expression
            var expression = specification.ToExpression();
            if (expression != null)
            {
                query = query.Where(expression);
            }


            // Includes all expression-based includes
            query = specification.Includes.Aggregate(query,
                (current, include) => current.Include(include));

            // Include any string-based include statements
            query = specification.IncludeStrings.Aggregate(query,
                (current, include) => current.Include(include));

            // Apply ordering if expressions are set
            if (specification.OrderBy != null)
            {
                query = query.OrderBy(specification.OrderBy);
            }
            else if (specification.OrderByDescending != null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);
            }
            else if (specification.OrderByDynamic != null)
            {
                query = query.OrderBy(specification.OrderByDynamic.Value.ordering, specification.OrderByDynamic.Value.args);
            }

            // Apply paging if enabled
            if (specification.IsPagingEnabled)
            {
                query = query.Skip((specification.PageNumber - 1) * specification.PageSize).Take(specification.PageSize);
            }
            return query;
        }

    }
}
