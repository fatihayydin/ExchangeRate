using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.CircuitBreaker;
using System.Net.Http.Headers;

namespace ExchangeRate.Infrastructure.ExternalServices
{
    public interface IExternalExchangeService
    {
        Task<ExchangeRatesModel?> GetLatest(string? exchangeBase, string? symbols = "");
    }

    public class ExternalExchangeService : IExternalExchangeService
    {
        private readonly string _baseUrl;
        private readonly string _apiKey;
        private readonly ILogger<ExternalExchangeService> _logger;
        private readonly Policy _policy;


        public async Task<ExchangeRatesModel?> GetLatest(string? exchangeBase, string? symbols = "")
        {
            if (string.IsNullOrWhiteSpace(exchangeBase))
            {
                exchangeBase = "EUR";
            }

            await _policy.Execute( async () => {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_baseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("apikey", _apiKey);

                    HttpResponseMessage response = await client.GetAsync($"fixer/latest?symbols={symbols}&base={exchangeBase}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = response.Content.ReadAsStringAsync().Result;

                        var result = JsonConvert.DeserializeObject<ExchangeRatesModel>(content);

                        return result;
                    }
                    else
                    {
                        _logger.LogCritical("Error from external exchange service", response.StatusCode);

                        throw new Exception(response.StatusCode.ToString());
                    }
                }
            });
        }

        public ExternalExchangeService(string baseUrl, string apiKey, ILogger<ExternalExchangeService> logger)
        {
            _baseUrl = baseUrl;
            _apiKey = apiKey;
            _logger = logger;

            _policy = Policy
                .Handle<Exception>()
                .WaitAndRetry(int.MaxValue, retryCount => TimeSpan.FromSeconds(Math.Pow(2, retryCount)),
                    (ex, span) =>
                    {
                        logger.LogWarning("Unable to update bulk listing operation task.", ex);
                    }); ;
        }
    }

    public record ExchangeRatesModel
    {
        public bool Success { get; set; }
        public string Base { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
    }

    public record RateModel
    {
        public string MyProperty { get; set; }
    }
}