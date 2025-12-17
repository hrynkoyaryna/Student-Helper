using System;
using System.Collections.Generic;
using System.Threading;

namespace StudentHelper.WPF.UI.Services
{
    /// <summary>
    /// Simple in-memory cache with expiration
    /// </summary>
    public class MemoryCache<TKey, TValue> where TKey : notnull
    {
        private readonly Dictionary<TKey, CacheEntry> _cache = new();
        private readonly ReaderWriterLockSlim _lock = new();
        private readonly TimeSpan _defaultExpiration;

        public MemoryCache(TimeSpan defaultExpiration)
        {
            _defaultExpiration = defaultExpiration;
        }

        /// <summary>
        /// Gets a value from cache
        /// </summary>
        public bool TryGet(TKey key, out TValue? value)
        {
            _lock.EnterReadLock();
            try
            {
                if (_cache.TryGetValue(key, out var entry))
                {
                    if (entry.ExpiresAt > DateTime.UtcNow)
                    {
                        value = entry.Value;
                        return true;
                    }
                    else
                    {
                        // Expired
                        _lock.ExitReadLock();
                        _lock.EnterWriteLock();
                        try
                        {
                            _cache.Remove(key);
                        }
                        finally
                        {
                            _lock.ExitWriteLock();
                            _lock.EnterReadLock();
                        }
                    }
                }

                value = default;
                return false;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Adds or updates a value in cache
        /// </summary>
        public void Set(TKey key, TValue value, TimeSpan? expiration = null)
        {
            var expiresAt = DateTime.UtcNow.Add(expiration ?? _defaultExpiration);

            _lock.EnterWriteLock();
            try
            {
                _cache[key] = new CacheEntry
                {
                    Value = value,
                    ExpiresAt = expiresAt
                };
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Removes a value from cache
        /// </summary>
        public void Remove(TKey key)
        {
            _lock.EnterWriteLock();
            try
            {
                _cache.Remove(key);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Clears all cache entries
        /// </summary>
        public void Clear()
        {
            _lock.EnterWriteLock();
            try
            {
                _cache.Clear();
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        private class CacheEntry
        {
            public TValue Value { get; set; } = default!;
            public DateTime ExpiresAt { get; set; }
        }
    }
}
