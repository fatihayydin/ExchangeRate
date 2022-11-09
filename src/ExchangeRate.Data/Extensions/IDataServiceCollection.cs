using Microsoft.Extensions.DependencyInjection;

namespace ExchangeRate.Data.Extensions
{
    public interface IDataServiceCollection
    {
        IServiceCollection ServiceCollection { get; }
    }
}
