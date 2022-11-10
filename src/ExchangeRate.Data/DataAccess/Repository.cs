using ExchangeRate.Abstraction.Data;
using Microsoft.EntityFrameworkCore;

namespace ExchangeRate.Data.DataAccess
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly DbContext _context;
        public Repository(DbContext context)
        {
            _context = context;
        }

        public async Task<TEntity> GetSingleAsync(object id, CancellationToken cancellationToken = default)
        {
            return await Entities.FindAsync(new[] { id }, cancellationToken);
        }

        public async Task MarkForInsertionAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await Entities.AddAsync(entity, cancellationToken);
        }

        public async Task MarkForDeletion(object id, CancellationToken cancellationToken = default)
        {
            var entity = await GetSingleAsync(id, cancellationToken);

            MarkForDeletion(entity);
        }

        public void MarkForDeletion(TEntity entity)
        {
            Entities.Remove(entity);
        }

        public async Task<IEnumerable<TEntity>> GetAsync(System.Linq.Expressions.Expression<Func<TEntity, bool>> filter)
        {
            return await Entities.Where(filter).ToListAsync();
        }

        public async Task<int> CountAsync(System.Linq.Expressions.Expression<Func<TEntity, bool>> filter = null)
        {
            return await Entities.Where(filter).CountAsync();
        }

        private DbSet<TEntity> Entities => _context.Set<TEntity>();
    }
}
