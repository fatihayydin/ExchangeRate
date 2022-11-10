using ExchangeRate.Application.Services;
using ExchangeRate.Infrastructure.Caching;
using ExchangeRate.Infrastructure.ExternalServices;
using Moq;
using Newtonsoft.Json;

namespace ExchangeRate.UnitTests
{
    public class ExchangeServiceTest
    {
        private readonly Mock<IExternalExchangeService> _mockExternalExchangeService;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly ExchangeRatesModel? _model;

        public ExchangeServiceTest()
        {
            _mockExternalExchangeService = new Mock<IExternalExchangeService>();
            _mockCacheService = new Mock<ICacheService>();
            _model = new ExchangeRatesModel
            {
                Success = true,
                Base = "EUR",
                Rates = new Dictionary<string, decimal>()
                {
                    {"TRY",2.22m },
                    {"GBP",3.22m },
                    {"USD",4.22m },
                }
            };
        }

        [Fact]
        public async void should_not_return_null_data_with_given_empty_parameters()
        {
            //Arrange
            _mockCacheService.Setup(x => x.GetFromString(It.IsAny<string>())).Returns(JsonConvert.SerializeObject(_model));

            //Act
            var sut = new ExchangeService(_mockExternalExchangeService.Object, _mockCacheService.Object);
            var returningData = await sut.Get("", "");

            //Assert
            Assert.NotNull(returningData);
        }

        [Fact]
        public async void should_return_all_data_with_given_empty_parameters()
        {
            //Arrange
            _mockCacheService.Setup(x => x.GetFromString(It.IsAny<string>())).Returns(JsonConvert.SerializeObject(_model));

            //Act
            var sut = new ExchangeService(_mockExternalExchangeService.Object, _mockCacheService.Object);
            var returningData = await sut.Get("", "");

            //Assert
            Assert.Equal(returningData.Rates.Count(), _model.Rates.Count());
        }

        [Fact]
        public async void should_return_only_try_with_given_try_symbol()
        {
            //Arrange
            _mockCacheService.Setup(x => x.GetFromString(It.IsAny<string>())).Returns(JsonConvert.SerializeObject(_model));

            //Act
            var sut = new ExchangeService(_mockExternalExchangeService.Object, _mockCacheService.Object);
            var returningData = await sut.Get("EUR", "TRY");

            //Assert
            Assert.Equal(1, returningData.Rates.Count());
            Assert.Equal("TRY", returningData.Rates.First().Key);
        }


        [Fact]
        public async void should_return_from_cached_data_if_there_is_cache_with_given_parameters()
        {
            Mock<ICacheService> _striclymockCacheService = new Mock<ICacheService>(MockBehavior.Strict);
            //Arrange
            _striclymockCacheService.Setup(x => x.GetFromString(It.IsAny<string>())).Returns(JsonConvert.SerializeObject(_model));

            //Act
            var sut = new ExchangeService(_mockExternalExchangeService.Object, _striclymockCacheService.Object);
            var returningData = await sut.Get("", "");

            //Assert
            Assert.NotNull(returningData);
        }


        [Fact]
        public async void should_add_cache_must_be_called_if_there_is_no_cache()
        {
            //Arrange
            _mockExternalExchangeService.Setup(x => x.GetLatest(It.IsAny<string>()))
                .Returns(Task.FromResult(_model));

            _mockCacheService.Setup(x => x.GetFromString(It.IsAny<string>())).Returns("");
            _mockCacheService.Setup(x => x.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()));

            //Act
            var sut = new ExchangeService(_mockExternalExchangeService.Object, _mockCacheService.Object);
            var returningData = await sut.Get("EUR", "TRY");

            //Assert
            _mockCacheService.Verify(x => x.Add(_model.Base, _model, It.IsAny<TimeSpan>()), Times.Once);
        }

        [Fact]
        public async void should_get_all_symbols_from_external_exchange_service_with_given_empty_parameters()
        {
            //Arrange
            _mockExternalExchangeService.Setup(x => x.GetLatest(It.IsAny<string>()))
                .Returns(Task.FromResult(_model));

            _mockCacheService.Setup(x => x.GetFromString(It.IsAny<string>())).Returns("");
            _mockCacheService.Setup(x => x.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()));

            //Act
            var sut = new ExchangeService(_mockExternalExchangeService.Object, _mockCacheService.Object);
            var returningData = await sut.Get("", "");

            //Assert
            Assert.Equal(returningData.Rates.Count(), _model.Rates.Count());
        }

        [Fact]
        public async void should_get_only_eur_from_external_exchange_service_with_given_gbp_symbol()
        {
            //Arrange
            _mockExternalExchangeService.Setup(x => x.GetLatest(It.IsAny<string>()))
                .Returns(Task.FromResult(_model));

            _mockCacheService.Setup(x => x.GetFromString(It.IsAny<string>())).Returns("");
            _mockCacheService.Setup(x => x.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()));

            //Act
            var sut = new ExchangeService(_mockExternalExchangeService.Object, _mockCacheService.Object);
            var returningData = await sut.Get("", "GBP");

            //Assert
            Assert.Equal("GBP", returningData.Rates.First().Key);
        }
    }
}