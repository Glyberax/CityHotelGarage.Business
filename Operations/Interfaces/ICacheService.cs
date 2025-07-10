using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace CityHotelGarage.Business.Operations.Interfaces;

public interface ICacheService
{
    T? Get<T>(string key);
    void Set<T>(string key, T value, TimeSpan? expiration = null);
    void Remove(string key);
    void RemoveByPattern(string pattern);
}