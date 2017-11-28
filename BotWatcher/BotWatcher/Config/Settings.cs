using Newtonsoft.Json;
using System.IO;

namespace BotWatcher.Config
{
    internal class Settings
    {
        private static string file = "./config.json";

        public string Token { get; set; } = "";
        public MonitorConfig[] Monitoring { get; set; } = new MonitorConfig[0];

        [JsonIgnore]
        private static FileInfo FullPath => new FileInfo(file);

        private static void EnsureDirectory()
        {
            if (!Directory.Exists(FullPath.Directory.FullName))
                Directory.CreateDirectory(FullPath.Directory.FullName);
        }

        public static Settings Load()
        {
            EnsureDirectory();
            if (!File.Exists(FullPath.FullName))
                Save(new Settings());
            return JsonConvert.DeserializeObject<Settings>(File.ReadAllText(FullPath.FullName));
        }

        private static void Save(Settings config)
        {
            EnsureDirectory();
            File.WriteAllText(FullPath.FullName, JsonConvert.SerializeObject(config, Formatting.Indented));
        }

        public void Save()
            => Save(this);
    }
}