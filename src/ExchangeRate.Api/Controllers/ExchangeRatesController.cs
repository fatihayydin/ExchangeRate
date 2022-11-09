using ExchangeRate.Api.Auth;
using ExchangeRate.Application.Services;
using ExchangeRate.Infrastructure.Exceptions;
using ExchangeRate.Infrastructure.ExternalServices;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(ExchangeRatesModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorBaseResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorBaseResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get([FromQuery] string? exchangeBase = "", [FromQuery] string? symbols = "")
        {
            _logger.LogInformation("Exchange Rates called!");

            return Ok(await _exchangeService.Get(exchangeBase, symbols));
        }
    }
}