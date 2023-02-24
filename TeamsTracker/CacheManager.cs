using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamsTracker
{
    internal class CacheManager
    {

        string CachePath { get; set; }

        public CacheManager(string _CachePath) 
        {
            CachePath = _CachePath;
        }

        public bool TryReadCache<T>(string file, out T data)
        {
            try
            {
                string jsonString = File.ReadAllText(System.IO.Path.Combine(CachePath, file));
                data = JsonConvert.DeserializeObject<T>(jsonString);
                return true;
            }
            catch { data = default; return false; }
        }

        public void WriteCache(string file, object data)
        {
            if (!File.Exists(System.IO.Path.Combine(CachePath, file)))
            {
                Directory.CreateDirectory(CachePath);
                File.Create(System.IO.Path.Combine(CachePath, file)).Close();
            }

            var jsonString = JsonConvert.SerializeObject(data);
            File.WriteAllText(System.IO.Path.Combine(CachePath, file), jsonString);
       }
    }
}
