using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FourTwenty.Core.Models;
using FourTwenty.Core.Specifications;

namespace FourTwenty.Core.Repositories
{
    public interface IAsyncRepository<T, in TKey> : IAsyncRepository<T> where T : BaseEntity<TKey>
    {
        Task<T?> GetByIdAsync(TKey id, CancellationToken ct = default);
        Task DeleteAsync(TKey key, CancellationToken ct = default);
    }

    public interface IAsyncRepository<T>
    {
        Task<T?> GetSingleBySpecAsync(ISpecification<T> spec, bool asNoTracking = true, CancellationToken ct = default);
        Task<IReadOnlyList<T>> ListAllAsync(bool asNoTracking = true, CancellationToken ct = default);
        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec, bool asNoTracking = true, CancellationToken ct = default);
        Task<T> AddAsync(T entity, CancellationToken ct = default);
        Task AddRangeAsync(IEnumerable<T> entity, CancellationToken ct = default);
        Task UpdateAsync(T entity, CancellationToken ct = default);
        Task DeleteAsync(T entity, CancellationToken ct = default);
        Task<int> CountAsync(ISpecification<T>? spec = null, CancellationToken ct = default);
        Task DeleteRangeAsync(IEnumerable<T> entity, CancellationToken ct = default);
    }
}


