using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public virtual T? GetById(TKey id)
        {
            return Set.Find(id);
        }

        public virtual async Task<T?> GetByIdAsync(TKey id, CancellationToken ct = default)
        {
            return await Set.FindAsync(new object[] { id! }, ct);
        }

        public virtual void Delete(TKey key)
        {
            var item = GetById(key);
            if (item != null)
            {
                Delete(item);
            }
        }

        public virtual async Task DeleteAsync(TKey key, CancellationToken ct = default)
        {
            var item = await GetByIdAsync(key, ct);
            if (item != null)
            {
                await DeleteAsync(item, ct);
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

        public virtual async Task<T?> GetSingleBySpecAsync(ISpecification<T> spec, bool asNoTracking = true, CancellationToken ct = default)
        {
            if (asNoTracking)
                return await ApplySpecification(spec).AsNoTracking().FirstOrDefaultAsync(cancellationToken: ct);
            return await ApplySpecification(spec).FirstOrDefaultAsync(cancellationToken: ct);
        }

        public virtual T? GetSingleBySpec(ISpecification<T> spec, bool asNoTracking = true) => List(spec, asNoTracking).FirstOrDefault();

        public virtual IQueryable<T> ListAll(bool asNoTracking = true)
        {
            if (asNoTracking)
                return Set.AsNoTracking();
            return Set;
        }

        public virtual async Task<IReadOnlyList<T>> ListAllAsync(bool asNoTracking = true, CancellationToken ct = default)
        {
            if (asNoTracking)
                return await Set.AsNoTracking().ToListAsync(cancellationToken: ct);

            return await Set.ToListAsync(cancellationToken: ct);
        }

        public virtual IQueryable<T> List(ISpecification<T> spec, bool asNoTracking = true)
        {
            if (asNoTracking)
                return ApplySpecification(spec).AsNoTracking();
            return ApplySpecification(spec);
        }

        public virtual async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec, bool asNoTracking = true, CancellationToken ct = default)
        {
            if (asNoTracking)
                return await ApplySpecification(spec).AsNoTracking().ToListAsync(cancellationToken: ct);
            return await ApplySpecification(spec).ToListAsync(cancellationToken: ct);
        }

        public virtual int Count(ISpecification<T>? spec = null)
        {
            if (spec != null)
                return ApplySpecification(spec).Count();
            return Set.Count();
        }

        public virtual async Task<int> CountAsync(ISpecification<T>? spec = null, CancellationToken ct = default)
        {
            if (spec != null)
                return await ApplySpecification(spec).CountAsync(cancellationToken: ct);
            return await Set.CountAsync(cancellationToken: ct);
        }

        public virtual T Add(T entity)
        {
            Set.Add(entity);
            DbContext.SaveChanges();

            return entity;
        }

        public virtual async Task<T> AddAsync(T entity, CancellationToken ct = default)
        {
            Set.Add(entity);
            await DbContext.SaveChangesAsync(ct);

            return entity;
        }

        public virtual void AddRange(IEnumerable<T> entity)
        {
            Set.AddRange(entity);
            DbContext.SaveChanges(); ;
        }

        public virtual async Task AddRangeAsync(IEnumerable<T> entity, CancellationToken ct = default)
        {
            await Set.AddRangeAsync(entity, ct);
            await DbContext.SaveChangesAsync(ct);
        }

        public virtual void Update(T entity)
        {
            DbContext.Entry(entity).State = EntityState.Modified;
            DbContext.SaveChanges();
        }

        public virtual async Task UpdateAsync(T entity, CancellationToken ct = default)
        {
            DbContext.Entry(entity).State = EntityState.Modified;
            await DbContext.SaveChangesAsync(ct);
        }

        public virtual void Delete(T entity)
        {
            Set.Remove(entity);
            DbContext.SaveChanges();
        }

        public virtual async Task DeleteAsync(T entity, CancellationToken ct = default)
        {
            Set.Remove(entity);
            await DbContext.SaveChangesAsync(ct);
        }

        public virtual void DeleteRange(IEnumerable<T> entity)
        {
            Set.RemoveRange(entity);
            DbContext.SaveChanges();
        }

        public virtual async Task DeleteRangeAsync(IEnumerable<T> entity, CancellationToken ct = default)
        {
            Set.RemoveRange(entity);
            await DbContext.SaveChangesAsync(ct);
        }

        protected virtual IQueryable<T> ApplySpecification(ISpecification<T> spec) => SpecificationEvaluator<T>.GetQuery(Set, spec);



    }
}
