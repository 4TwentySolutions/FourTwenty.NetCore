using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FourTwenty.Core.Extensions
{
    public static class LinqExtensions
    {
        public static IQueryable<T> DynamicContains<T, TProperty>(
            this IQueryable<T> query,
            string property,
            IList<TProperty> items)
        {
            var pe = Expression.Parameter(typeof(T));
            var me = Expression.Property(pe, property);

            var ce = Expression.Constant(items);
            var call = Expression.Call(typeof(Enumerable), "Contains", new[] { me.Type }, ce, me);
            var lambda = Expression.Lambda<Func<T, bool>>(call, pe);
            return query.Where(lambda);
        }
        public static IQueryable<T> DynamicContains<T, TProperty>(
            this IQueryable<T> query,
            string property,
            IEnumerable<TProperty> items, Type itemType)
        {
            var pe = Expression.Parameter(typeof(T));
            var me = Expression.Property(pe, property);
            var ce = Expression.Constant(items, itemType);
            var call = Expression.Call(typeof(Enumerable), "Contains", new[] { me.Type }, ce, me);
            var lambda = Expression.Lambda<Func<T, bool>>(call, pe);
            return query.Where(lambda);
        }

        public static IQueryable<T> DistinctBy<T, TKey>(this IQueryable<T> items, Expression<Func<T, TKey>> property)
        {
            return items.GroupBy(property).Select(x => x.First());
        }




        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static IEnumerable<T> Where<T, TPropT>(this IEnumerable<T> self, string propertyName, Expression<Func<TPropT, bool>> predicate)
        {
            var paramExpr = Expression.Parameter(typeof(T));
            Expression body = paramExpr;
            foreach (var member in propertyName.Split('.'))
            {
                body = Expression.PropertyOrField(body, member);
            }
            return self.Where(Expression.Lambda<Func<T, bool>>(Expression.Invoke(predicate, body), paramExpr).Compile());
        }


        public static IEnumerable<IEnumerable<T>> Split<T>(this T[] array, int size)
        {
            for (var i = 0; i < (float)array.Length / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }

        /// <summary>
        /// Splits a <see cref="List{T}"/> into multiple chunks.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list to be chunked.</param>
        /// <param name="chunkSize">The size of each chunk.</param>
        /// <returns>A list of chunks.</returns>
        public static List<List<T>> SplitIntoChunks<T>(this List<T> list, int chunkSize)
        {
            if (chunkSize <= 0)
            {
                throw new ArgumentException("chunkSize must be greater than 0.");
            }

            List<List<T>> retVal = new List<List<T>>();
            int index = 0;
            while (index < list.Count)
            {
                int count = list.Count - index > chunkSize ? chunkSize : list.Count - index;
                retVal.Add(list.GetRange(index, count));

                index += chunkSize;
            }

            return retVal;
        }
    }
}
