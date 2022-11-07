using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRate.Abstraction.Data
{
    public interface IRepository<TEntity>
    {
        Task<TEntity> GetSingleAsync(object id, CancellationToken cancellationToken = default);
        Task MarkForInsertionAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task MarkForDeletion(object id, CancellationToken cancellationToken = default);
        void MarkForDeletion(TEntity entity);
    }
}
