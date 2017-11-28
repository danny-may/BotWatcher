using Discord;
using System.Collections.Generic;

namespace BotWatcher.Config
{
    public class MonitorConfig
    {
        public ulong UserId { get; set; }
        public Dictionary<UserStatus, ActionResponse> StatusHandlers { get; set; } = new Dictionary<UserStatus, ActionResponse>();
        public DelayConfig DelayCheck { get; set; }
    }
}