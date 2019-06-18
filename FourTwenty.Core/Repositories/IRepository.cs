using System.Collections.Generic;
using FourTwenty.Core.Interfaces;
using FourTwenty.Core.Models;

namespace FourTwenty.Core.Repositories
{
    public interface IRepository<T, in TKey> : IRepository<T> where T : BaseEntity<TKey>
    {
        T GetById(TKey id);
        void Delete(TKey key);
    }

    public interface IRepository<T>
    {
        T GetSingleBySpec(ISpecification<T> spec);
        IEnumerable<T> ListAll();
        IEnumerable<T> List(ISpecification<T> spec);
        T Add(T entity);
        void AddRange(IEnumerable<T> entity);
        void Update(T entity);
        void Delete(T entity);
        int Count(ISpecification<T> spec = null);
        void DeleteRange(IEnumerable<T> entity);
    }
}
