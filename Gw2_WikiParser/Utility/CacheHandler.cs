using System;
using System.Collections.Generic;
using System.Text;

namespace Gw2_WikiParser.Utility
{
    public class CacheHandler<T>
    {
        private readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string _name;

        public CacheHandler(string name)
        {
            _name = name;
            InternalCache.Instance.Add(_name, new Dictionary<string, T>());
        }

        public void Add(string key, T obj)
        {
            Dictionary<string, T> cacheSector = InternalCache.Instance.Get<Dictionary<string, T>>(_name);
            if (!cacheSector.ContainsKey(key))
            {
                cacheSector.Add(key, obj);
            }
        }

        public void Set(string key, T obj)
        {
            Dictionary<string, T> cacheSector = InternalCache.Instance.Get<Dictionary<string, T>>(_name);
            if (!cacheSector.ContainsKey(key))
            {
                cacheSector.Add(key, default(T));
            }
            cacheSector[key] = obj;
        }

        public T Get(string key)
        {
            Dictionary<string, T> cacheSector = InternalCache.Instance.Get<Dictionary<string, T>>(_name);
            if (cacheSector.ContainsKey(key))
            {
                return cacheSector[key];
            }
            return default(T);
        }

        public void Remove(string key)
        {
            Dictionary<string, T> cacheSector = InternalCache.Instance.Get<Dictionary<string, T>>(_name);
            if (cacheSector.ContainsKey(key))
            {
                cacheSector.Remove(key);
            }
        }

        public bool Contains(string key)
        {
            Dictionary<string, T> cacheSector = InternalCache.Instance.Get<Dictionary<string, T>>(_name);
            return cacheSector.ContainsKey(key);
        }

        public void Import(string path)
        {
            InternalCache.Instance.Import<T>(path, _name);
        }
    }

}
