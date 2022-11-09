namespace ExchangeRate.Abstraction.Data
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
        int SaveChanges(bool acceptAllChangesOnSuccess);
        bool BeginNewTransaction();
        bool RollBackTransaction();
        Task<TEntity> AttachAsync<TEntity>(object id) where TEntity : class;
    }
}
