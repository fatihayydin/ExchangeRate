using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ExchangeRate.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IDataServiceCollection AddDataContext<TDbContext>(this IServiceCollection serviceCollection,
            Action<DbContextOptionsBuilder> optionsAction = null) where TDbContext : DbContext
        {
            serviceCollection.AddDbContextPool<TDbContext>(optionsAction);
            serviceCollection.AddScoped<DbContext>(provider => provider.GetService<TDbContext>());

            return new DataServiceCollection(serviceCollection);
        }
    }
}
