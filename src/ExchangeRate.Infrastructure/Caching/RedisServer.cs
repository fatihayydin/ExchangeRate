using StackExchange.Redis;


namespace ExchangeRate.Infrastructure.Caching
{
    public class RedisServer
    {
        public RedisServer(string hosts, string password)
        {
            var connectionMultiplexer = ConnectionMultiplexer.Connect(CreateRedisConfigurationString(hosts, password));
            Database = connectionMultiplexer.GetDatabase(0);
        }

        public IDatabase Database { get; }

        private ConfigurationOptions CreateRedisConfigurationString(string hosts, string password)
        {
            var options = ConfigurationOptions.Parse(hosts);
            options.Password = password;
            //Todo: Can get external
            options.ConnectTimeout = 5000;
            options.ConnectRetry = 2;

            return options;
        }
    }
}
