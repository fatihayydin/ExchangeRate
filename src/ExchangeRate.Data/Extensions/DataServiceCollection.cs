using Microsoft.Extensions.DependencyInjection;

namespace ExchangeRate.Data.Extensions
{
    public class DataServiceCollection : IDataServiceCollection
    {
        public DataServiceCollection(IServiceCollection serviceCollection)
        {
            ServiceCollection = serviceCollection;
        }

        public IServiceCollection ServiceCollection { get; protected set; }
    }
}
