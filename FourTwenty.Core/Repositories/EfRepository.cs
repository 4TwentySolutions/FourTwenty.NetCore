using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FourTwenty.Core.Interfaces;
using FourTwenty.Core.Models;
using FourTwenty.Core.Specifications;
using Microsoft.EntityFrameworkCore;

namespace FourTwenty.Core.Repositories
{
    /// <summary>
    /// "There's some repetition here - couldn't we have some the sync methods call the async?"
    /// https://blogs.msdn.microsoft.com/pfxteam/2012/04/13/should-i-expose-synchronous-wrappers-for-asynchronous-methods/
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class EfRepository<T, TKey> : EfRepository<T>, IRepository<T, TKey>, IAsyncRepository<T, TKey> where T : BaseEntity<TKey>
    {
        public EfRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public virtual T GetById(TKey id)
        {
            return Set.Find(id);
        }

        public virtual async Task<T> GetByIdAsync(TKey id)
        {
            return await Set.FindAsync(id);
        }

        public virtual void Delete(TKey key)
        {
            var item = GetById(key);
            if (item != null)
            {
                Delete(item);
            }
        }

        public virtual async Task DeleteAsync(TKey key)
        {
            var item = GetById(key);
            if (item != null)
            {
                await DeleteAsync(item);
            }
        }
    }

    public class EfRepository<T> : IRepository<T>, IAsyncRepository<T> where T : class
    {
        protected readonly DbContext DbContext;
        protected readonly DbSet<T> Set;

        public EfRepository(DbContext dbContext)
        {
            DbContext = dbContext;
            Set = dbContext.Set<T>();
        }

        public virtual async Task<T> GetSingleBySpecAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).AsNoTracking().FirstOrDefaultAsync();
        }

        public virtual T GetSingleBySpec(ISpecification<T> spec)
        {
            return List(spec).FirstOrDefault();
        }

        public virtual IEnumerable<T> ListAll()
        {
            return Set.AsNoTracking().AsEnumerable();
        }

        public virtual async Task<IReadOnlyList<T>> ListAllAsync()
        {
            return await Set.AsNoTracking().ToListAsync();
        }

        public virtual IEnumerable<T> List(ISpecification<T> spec)
        {
            return ApplySpecification(spec).AsNoTracking().AsEnumerable();
        }

        public virtual async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).AsNoTracking().ToListAsync();
        }

        public virtual int Count(ISpecification<T> spec = null)
        {
            if (spec != null)
                return ApplySpecification(spec).Count();
            return Set.Count();
        }

        public virtual async Task<int> CountAsync(ISpecification<T> spec = null)
        {
            if (spec != null)
                return await ApplySpecification(spec).CountAsync();
            return await Set.CountAsync();
        }

        public virtual T Add(T entity)
        {
            Set.Add(entity);
            DbContext.SaveChanges();

            return entity;
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            Set.Add(entity);
            await DbContext.SaveChangesAsync();

            return entity;
        }

        public virtual void AddRange(IEnumerable<T> entity)
        {
            Set.AddRange(entity);
            DbContext.SaveChanges(); ;
        }

        public virtual async Task AddRangeAsync(IEnumerable<T> entity)
        {
            await Set.AddRangeAsync(entity);
            await DbContext.SaveChangesAsync();
        }

        public virtual void Update(T entity)
        {
            DbContext.Entry(entity).State = EntityState.Modified;
            DbContext.SaveChanges();
        }

        public virtual async Task UpdateAsync(T entity)
        {
            DbContext.Entry(entity).State = EntityState.Modified;
            await DbContext.SaveChangesAsync();
        }

        public virtual void Delete(T entity)
        {
            Set.Remove(entity);
            DbContext.SaveChanges();
        }

        public virtual async Task DeleteAsync(T entity)
        {
            Set.Remove(entity);
            await DbContext.SaveChangesAsync();
        }

        public virtual void DeleteRange(IEnumerable<T> entity)
        {
            Set.RemoveRange(entity);
            DbContext.SaveChanges();
        }

        public virtual async Task DeleteRangeAsync(IEnumerable<T> entity)
        {
            Set.RemoveRange(entity);
            await DbContext.SaveChangesAsync();
        }

        protected virtual IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(Set, spec);
        }


    }
}
