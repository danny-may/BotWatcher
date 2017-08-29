using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace BotWatcher.Services
{
    public delegate Task UserStatusChanged(SocketUser user, UserStatus oldStatus, UserStatus newStatus);

    class MonitorService
    {
        private DiscordSocketClient _discord;
        private Configuration _config;
        private MessagingService _messaging;

        public event UserStatusChanged UserStatusChanged;

        public MonitorService(DiscordSocketClient discord, Configuration config, MessagingService messaging)
        {
            _discord = discord;
            _config = config;
            _messaging = messaging;

            _discord.GuildMemberUpdated += UserUpdated;
            _discord.UserUpdated += UserUpdated;
            UserStatusChanged += NotifyListeners;
        }

        private async Task UserUpdated(SocketUser oldUser, SocketUser newUser)
        {
            if (oldUser.Status != newUser.Status)
                await UserStatusChanged?.Invoke(newUser, oldUser.Status, newUser.Status);
        }

        private async Task NotifyListeners(SocketUser user, UserStatus oldStatus, UserStatus newStatus)
        {
            if (newStatus == UserStatus.Offline && _config.Tracking.TryGetValue(user.Id, out var listeners))
                foreach (var listener in listeners)
                    await _messaging.DMUser(listener, $"⚠️ The user `{user.Username}#{user.Discriminator} ({user.Id})` has just gone offline");
        }
    }
}
