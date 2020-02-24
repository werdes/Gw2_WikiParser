using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Gw2_WikiParser.Utility
{
    public class InternalCache
    {
        private static Lazy<InternalCache> _instance = new Lazy<InternalCache>(() => new InternalCache());
        private Dictionary<string, object> _cache = new Dictionary<string, object>();
        public static InternalCache Instance
        {
            get => _instance.Value;
        }

        private InternalCache()
        {

        }

        public void Add<T>(string key, T obj)
        {
            if (!_cache.ContainsKey(key))
            {
                _cache.Add(key, obj);
            }
        }

        public T Get<T>(string key)
        {
            if (_cache.ContainsKey(key) && _cache[key] is T)
            {
                return (T)_cache[key];
            }
            return default(T);
        }
        public void Set<T>(string key, T obj)
        {
            if (!_cache.ContainsKey(key))
            {
                _cache.Add(key, default(T));
            }
            _cache[key] = obj;
        }


        public void Export(string path)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Create, true))
                {
                    foreach (string key in _cache.Keys)
                    {
                        string json = JsonConvert.SerializeObject(_cache[key], Formatting.None);
                        ZipArchiveEntry entry = archive.CreateEntry($"{key}.json");

                        using (Stream entryStream = entry.Open())
                        {
                            using (StreamWriter writer = new StreamWriter(entryStream, Encoding.UTF8))
                            {
                                writer.Write(json);
                                writer.Close();
                            }
                            entryStream.Close();
                        }
                    }
                }

                stream.Flush();
                stream.Close();

                File.WriteAllBytes(path, stream.ToArray());
            }
        }

        public void Import<T>(string path, string key)
        {
            if (File.Exists(path))
            {
                using (ZipArchive archive = ZipFile.OpenRead(path))
                {
                    ZipArchiveEntry entry = archive.GetEntry($"{key}.json");
                    using (Stream entryStream = entry.Open())
                    {
                        using (StreamReader reader = new StreamReader(entryStream, Encoding.UTF8))
                        {
                            string content = reader.ReadToEnd();
                            T obj = JsonConvert.DeserializeObject<T>(content);
                            Set(key, obj);
                            reader.Close();
                        }
                        entryStream.Close();
                    }
                }
            }
        }
    }
}
