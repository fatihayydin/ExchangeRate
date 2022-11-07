using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRate.Data.Extensions
{
    public interface IDataServiceCollection
    {
        IServiceCollection ServiceCollection { get; }
    }
}
