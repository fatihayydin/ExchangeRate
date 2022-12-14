using ExchangeRate.Infrastructure.Caching;
using ExchangeRate.Infrastructure.Extensions;
using ExchangeRate.Infrastructure.ExternalServices;
using Newtonsoft.Json;

namespace ExchangeRate.Application.Services
{
    public interface IExchangeService
    {
        Task<ExchangeRatesModel?> Get(string? exchangeBase, string? symbols);
    }
    public class ExchangeService : IExchangeService
    {
        private readonly IExternalExchangeService _externalExchangeService;
        private readonly ICacheService _cacheService;

        public ExchangeService(IExternalExchangeService externalExchangeService, ICacheService cacheService)
        {
            _externalExchangeService = externalExchangeService;
            _cacheService = cacheService;
        }

        public async Task<ExchangeRatesModel?> Get(string? exchangeBase, string? symbols)
        {
            if (string.IsNullOrWhiteSpace(exchangeBase))
            {
                exchangeBase = "EUR";
            }

            var exchangeRatesFromCache = _cacheService.GetFromString(exchangeBase);

            if (!String.IsNullOrEmpty(exchangeRatesFromCache))
            {
                var allData = JsonConvert.DeserializeObject<ExchangeRatesModel>(exchangeRatesFromCache);

                FilterValuesForSymbols(ref allData, symbols);

                return allData;
            }

            var exchangeRates = await _externalExchangeService.GetLatest(exchangeBase);

            _cacheService.Add(exchangeBase, exchangeRates, TimeSpan.FromMinutes(30));

            FilterValuesForSymbols(ref exchangeRates, symbols);

            return exchangeRates;
        }

        private void FilterValuesForSymbols(ref ExchangeRatesModel? model, string symbols)
        {
            var filteredSymbolsData = model.Rates.Where(dic => symbols.ReplaceWhitespace().Split(',').Any(filter => dic.Key.Contains(filter))).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            model.Rates = filteredSymbolsData;
        }

    }
}
