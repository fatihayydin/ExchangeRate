using ExchangeRate.Api.Auth;
using ExchangeRate.Application.ExternalServices;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeRate.Api.Controllers
{
    //Todo: Will be uncommented before sending!!
    //[Authorize]
    [ApiController]
    [Route("exchange-rates")]
    public class ExchangeRatesController : ControllerBase
    {

        private readonly ILogger<ExchangeRatesController> _logger;
        private readonly IExternalExchangeService _externalExchangeService;

        public ExchangeRatesController(ILogger<ExchangeRatesController> logger, IExternalExchangeService externalExchangeService)
        {
            _logger = logger;
            _externalExchangeService = externalExchangeService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery] string? exchangeBase = "", [FromQuery] string? symbols = "")
        {
            _logger.LogInformation("Exchange Rates called!");

            var result = await _externalExchangeService.GetLatest(exchangeBase, symbols);

            return Ok(result);
        }
    }
}