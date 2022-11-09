namespace ExchangeRate.Infrastructure.Caching
{
    public interface ICacheService
    {
        string? GetFromString(string key);
        void Add(string key, object data, TimeSpan? expiry = null);
        void Remove(string key);
        // void Clear();
        bool Any(string key);
    }
}
