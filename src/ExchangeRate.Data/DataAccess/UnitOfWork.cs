using ExchangeRate.Abstraction.Data;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;

namespace ExchangeRate.Data.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;
        private readonly IServiceProvider _serviceProvider;
        private IDbContextTransaction _transaction;

        public UnitOfWork(DbContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            _serviceProvider = serviceProvider;
        }


        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            return (IRepository<TEntity>)_serviceProvider.GetService(typeof(IRepository<TEntity>));
        }


        public bool BeginNewTransaction()
        {
            try
            {
                _transaction = _context.Database.BeginTransaction();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool RollBackTransaction()
        {
            try
            {
                _transaction.Rollback();
                _transaction = null;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<TEntity> AttachAsync<TEntity>(object id) where TEntity : class
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            int result;

            try
            {
                result = _context.SaveChanges(acceptAllChangesOnSuccess);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }

            return result;
        }


        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var transaction = _transaction ?? _context.Database.BeginTransaction();
            var count = 0;

            using (transaction)
            {
                try
                {
                    count = await _context.SaveChangesAsync(cancellationToken);
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message, e);
                }
            }

            return count;
        }

        private bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }

            this._disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
