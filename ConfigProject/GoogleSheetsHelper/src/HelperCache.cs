using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class HelperCache
{   
    public Dictionary<string, string> FilePathToSheetID = new Dictionary<string, string>();

    [JsonIgnore]
    private string _cache_path;

    public static HelperCache LoadOrCreate (string cache_path)
    {
        if (File.Exists(cache_path))
        {
            var cached_info = File.ReadAllText(cache_path);
            try
            {
                var cache = JsonConvert.DeserializeObject<HelperCache>(cached_info);
                cache._cache_path = cache_path;
                return cache;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // File.Delete(CacheFilePath);
            }
        }

        return new HelperCache
        {
            _cache_path = cache_path,
        };
    }

    public void Save ()
    {
        var dir_path = Path.GetDirectoryName(_cache_path);
        if (!Directory.Exists(dir_path))
            Directory.CreateDirectory(dir_path);

        File.WriteAllText(_cache_path, JsonConvert.SerializeObject(this, Formatting.Indented));
    }
}
