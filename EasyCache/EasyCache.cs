using System;
using System.Collections.Generic;
using System.IO;
using Jil;
using Microsoft.Extensions.Caching.Distributed;

namespace EasyCache
{
    public class EasyCache: IEasyCache
    {
        private class ExceptionMessages
        {
            public const string KeyMustBeSupplied = "A key must be supplied";
        }
        
        private readonly IDistributedCache _distributedCache;
        private readonly ICompressionService _compressionService;

        public EasyCache(IDistributedCache distributedCache, ICompressionService compressionService)
        {
            _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
            _compressionService = compressionService ?? throw new ArgumentNullException(nameof(compressionService));
        }
        public T GetWithCompression<T>(string key)
        {
            var compressed = _distributedCache.Get(key);
            if (compressed == null) return default(T);
            var uncompressed = _compressionService.Decompress(compressed);
            return Deserialize<T>(uncompressed);
        }

        public void SetWithCompression<T>(string key, T value)
        {
            var data = Serialize<T>(value);
            var compressed = _compressionService.Compress(data);
            _distributedCache.Set(key, compressed);
        }
        public T Get<T>(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new InvalidOperationException(ExceptionMessages.KeyMustBeSupplied);
            var data = _distributedCache.GetString(key);
            if (data == null) return default(T);
            return Deserialize<T>(data);
        }

        public void Set<T>(string key, T value)
        {
            if (string.IsNullOrEmpty(key)) throw new InvalidOperationException(ExceptionMessages.KeyMustBeSupplied);
            var data = Serialize<T>(value);
            _distributedCache.SetString(key, data);
        }

        public void Remove(string key)
        {
            _distributedCache.Remove(key);
        }

        private string Serialize<T>(T value)
        {
            using (var output = new StringWriter())
            {
                JSON.Serialize(
                    value,
                    output
                );
                return output.ToString();
            }
        }

        private T Deserialize<T>(string value)
        {
            return JSON.Deserialize<T>(value);
        }
    }
}
