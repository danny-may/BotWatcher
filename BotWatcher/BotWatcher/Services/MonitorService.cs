using BotWatcher.Config;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BotWatcher.Services
{
    public delegate Task UserStatusChanged(SocketUser user, UserStatus oldStatus, UserStatus newStatus);

    internal class MonitorService
    {
        private DiscordSocketClient _discord;
        private Settings _config;
        private MessagingService _messaging;
        private List<Action> _delayChecks;

        public event UserStatusChanged UserStatusChanged;

        public MonitorService(DiscordSocketClient discord, Settings config, MessagingService messaging)
        {
            _discord = discord;
            _config = config;
            _messaging = messaging;
            _delayChecks = new List<Action>();

            _discord.GuildMemberUpdated += UserUpdated;
            _discord.UserUpdated += UserUpdated;
            UserStatusChanged += NotifyListeners;

            //foreach (var monitor in _config.Monitoring)
            //    if (monitor.DelayCheck != null)
            //        _delayChecks.Add(CreateChecker(monitor));
        }

        //private Action CreateChecker(MonitorConfig monitor)
        //{
        //    bool hasReplied = false;

        //    Task MessageRecieved(SocketMessage message)
        //    {
        //        hasReplied = true;
        //        _discord.MessageReceived -= MessageRecieved;
        //        return Task.CompletedTask;
        //    }

        //    return () =>
        //    {
        //        _messaging.DMUser(monitor.UserId, monitor.DelayCheck?.Message ?? "ping");
        //        _discord.MessageReceived += MessageRecieved;
        //        Task.Run(async () =>
        //        {
        //            await Task.Delay(monitor.DelayCheck.MaxDelay);
        //            if (hasReplied)
        //                return;

        //            var user = _discord.GetUser(monitor.UserId);
        //            await RunAlerts(monitor.DelayCheck.OnSlow, $"⚠️ The user `{user.Username}#{user.Discriminator} ({user.Id})` is running slow (> {monitor.DelayCheck.MaxDelay}ms delay)");
        //        });
        //    };
        //}

        private async Task UserUpdated(SocketUser oldUser, SocketUser newUser)
        {
            try
            {
                if (oldUser.Status != newUser.Status)
                    await UserStatusChanged?.Invoke(newUser, oldUser.Status, newUser.Status);
            }
            catch
            {
            }
        }

        private async Task NotifyListeners(SocketUser user, UserStatus oldStatus, UserStatus newStatus)
        {
            var monitor = _config.Monitoring.FirstOrDefault(m => m.UserId == user.Id);
            if (monitor == null || !monitor.StatusHandlers.TryGetValue(newStatus, out var actions))
                return;

            await RunAlerts(actions, $"⚠️ The user `{user.Username}#{user.Discriminator} ({user.Id})` has just gone offline");
        }

        private async Task RunAlerts(ActionResponse actions, string message)
        {
            foreach (var userid in actions.AlertUsers)
                await _messaging.DMUser(userid, message);

            if (File.Exists(actions.CommandFile))
                Process.Start(actions.CommandFile);
        }
    }
}