using System.Collections.Generic;
using System.Linq;
using FourTwenty.Core.Models;
using FourTwenty.Core.Specifications;

namespace FourTwenty.Core.Repositories
{
    public interface IRepository<T, in TKey> : IRepository<T> where T : BaseEntity<TKey>
    {
        T? GetById(TKey id);
        void Delete(TKey key);
    }

    public interface IRepository<T>
    {
        T? GetSingleBySpec(ISpecification<T> spec, bool asNoTracking = true);
        IQueryable<T> ListAll(bool asNoTracking = true);
        IQueryable<T> List(ISpecification<T> spec, bool asNoTracking = true);
        T Add(T entity);
        void AddRange(IEnumerable<T> entity);
        void Update(T entity);
        void Delete(T entity);
        int Count(ISpecification<T>? spec = null);
        void DeleteRange(IEnumerable<T> entity);
    }
}
