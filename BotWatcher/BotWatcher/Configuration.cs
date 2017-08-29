using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace BotWatcher
{
    class Configuration
    {
        private static string file = "./config.json";

        public string Token { get; set; } = "";
        public Dictionary<ulong, ulong[]> Tracking { get; set; } = new Dictionary<ulong, ulong[]>();

        [JsonIgnore]
        private static FileInfo FullPath => new FileInfo(Path.Combine(AppContext.BaseDirectory, file));

        private static void EnsureDirectory()
        {
            if (!Directory.Exists(FullPath.Directory.FullName))
                Directory.CreateDirectory(FullPath.Directory.FullName);
        }

        public static Configuration Load()
        {
            EnsureDirectory();
            if (!File.Exists(FullPath.FullName))
                Save(new Configuration());
            return JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(FullPath.FullName));
        }

        private static void Save(Configuration config)
        {
            EnsureDirectory();
            File.WriteAllText(FullPath.FullName, JsonConvert.SerializeObject(config, Formatting.Indented));
        }

        public void Save()
            => Save(this);
    }
}