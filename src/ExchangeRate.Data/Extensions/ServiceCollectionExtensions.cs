using ExchangeRate.Abstraction.Data;
using ExchangeRate.Data.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
