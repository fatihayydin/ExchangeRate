using ExchangeRate.Api.Auth;
using ExchangeRate.Application.Services;
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
        private readonly IExchangeService _exchangeService;

        public ExchangeRatesController(ILogger<ExchangeRatesController> logger, IExchangeService exchangeService)
        {
            _logger = logger;
            _exchangeService = exchangeService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery] string? exchangeBase = "", [FromQuery] string? symbols = "")
        {
            _logger.LogInformation("Exchange Rates called!");

            return Ok(await _exchangeService.Get(exchangeBase, symbols));
        }
    }
}