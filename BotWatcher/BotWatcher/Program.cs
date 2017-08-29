using BotWatcher.Services;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;


namespace BotWatcher
{
    class Program
    {
        static void Main(string[] args)
            => new Program().StartAsync().GetAwaiter().GetResult();

        public async Task StartAsync()
        {
            var config = Configuration.Load();

            var services = new ServiceCollection()
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Debug,
                    MessageCacheSize = 1000
                }))
                .AddSingleton<StartupService>()
                .AddSingleton<LoggingService>()
                .AddSingleton<MessagingService>()
                .AddSingleton<MonitorService>()
                .AddSingleton(config);

            var provider = services.BuildServiceProvider();

            provider.GetRequiredService<LoggingService>();
            await provider.GetRequiredService<StartupService>().StartAsync();
            provider.GetRequiredService<MonitorService>();

            await Task.Delay(-1);
        }
    }
}