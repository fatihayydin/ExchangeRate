using ExchangeRate.Api.Auth;
using ExchangeRate.Infrastructure.Caching;
using ExchangeRate.Infrastructure.Extensions;
using ExchangeRate.Infrastructure.ExternalServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace ExchangeRate.Api.Controllers
{
    //Todo: Will be uncommented before sending!!
    [Authorize]
    [ApiController]
    [Route("exchange-rates")]
    public class ExchangeRatesController : ControllerBase
    {

        private readonly ILogger<ExchangeRatesController> _logger;
        private readonly IExternalExchangeService _externalExchangeService;
        private readonly ICacheService _cacheService;

        public ExchangeRatesController(ILogger<ExchangeRatesController> logger, IExternalExchangeService externalExchangeService, ICacheService cacheService)
        {
            _logger = logger;
            _externalExchangeService = externalExchangeService;
            _cacheService = cacheService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery] string? exchangeBase = "", [FromQuery] string? symbols = "")
        {
            _logger.LogInformation("Exchange Rates called!");

            if (string.IsNullOrWhiteSpace(exchangeBase))
            {
                exchangeBase = "EUR";
            }

            //Todo: can be stored as JSON!?
            var exchangeRatesFromCache = _cacheService.GetFromString(exchangeBase);

            if(!String.IsNullOrEmpty(exchangeRatesFromCache))
            {
                var allData = JsonConvert.DeserializeObject<ExchangeRatesModel>(exchangeRatesFromCache);
                //Todo: symbols will be filtered!
                return Ok(allData);
            }

            var exchangeRates = await _externalExchangeService.GetLatest(exchangeBase);

            _cacheService.Add(exchangeBase, exchangeRates, TimeSpan.FromMinutes(30));

            return Ok(exchangeRates);
        }
    }
}