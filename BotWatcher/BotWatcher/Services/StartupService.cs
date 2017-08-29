using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace BotWatcher.Services
{
    class StartupService
    {
        private readonly DiscordSocketClient _discord;
        private readonly Configuration _config;

        public StartupService(
            DiscordSocketClient discord,
            Configuration config)
        {
            _config = config;
            _discord = discord;
        }

        public async Task StartAsync()
        {
            string discordToken = _config.Token;
            if (string.IsNullOrWhiteSpace(discordToken))
                throw new Exception("Please enter your bot's token into the `_configuration.json` file found in the applications root directory.");

            await _discord.LoginAsync(TokenType.Bot, discordToken);
            await _discord.StartAsync();
        }
    }
}
