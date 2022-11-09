using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRate.Infrastructure.Caching
{
    public class RedisCacheService : ICacheService
    {
        private readonly RedisServer _redisServer;

        public RedisCacheService(RedisServer redisServer)
        {
            _redisServer = redisServer;
        }

        public void Add(string key, object data, TimeSpan? expiry = null)
        {
            var jsonData = JsonConvert.SerializeObject(data);
            _redisServer.Database.StringSet(key, jsonData, expiry ?? TimeSpan.FromHours(1));
        }

        public bool Any(string key)
        {
            return _redisServer.Database.KeyExists(key);
        }

        public string? GetFromString(string key)
        {
            if (Any(key))
            {
                return _redisServer.Database.StringGet(key);
            }

            return null;

        }

        public void Remove(string key)
        {
            _redisServer.Database.KeyDelete(key);
        }

        // public void Clear()
        // {
        //     _redisServer.FlushDatabase();
        // }
    }
}
