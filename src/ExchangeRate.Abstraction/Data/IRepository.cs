using System.Linq.Expressions;

namespace ExchangeRate.Abstraction.Data
{
    public interface IRepository<TEntity>
    {
        Task<TEntity> GetSingleAsync(object id, CancellationToken cancellationToken = default);
        Task MarkForInsertionAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task MarkForDeletion(object id, CancellationToken cancellationToken = default);
        void MarkForDeletion(TEntity entity);
        Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter);
        Task<int> CountAsync(System.Linq.Expressions.Expression<Func<TEntity, bool>> filter = null);
    }
}
