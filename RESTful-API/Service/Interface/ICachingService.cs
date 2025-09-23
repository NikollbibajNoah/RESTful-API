﻿namespace RESTful.Service.Interface;

public interface ICachingService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null);
    Task RemoveAsync(string key);
    Task RemoveByPatternAsync(string pattern);
}