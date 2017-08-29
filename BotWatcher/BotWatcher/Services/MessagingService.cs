using Discord;
using Discord.Net;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace BotWatcher.Services
{
    class MessagingService
    {
        private DiscordSocketClient _discord;

        public MessagingService(DiscordSocketClient discord)
        {
            _discord = discord;
        }

        public async Task<IUserMessage> SendAsync(IMessageChannel channel, IUser user, string text, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            try
            {
                try
                {
                    return await channel.SendMessageAsync(text, isTTS, embed, options);
                }
                catch (HttpException ex) when (ex.DiscordCode == 50013 || ex.DiscordCode == 50001)
                {
                    return await (await user.GetOrCreateDMChannelAsync()).SendMessageAsync(text, isTTS, embed, options);
                }
            }
            catch
            {
                return null;
            }
        }

        public Task<IUserMessage> DMUser(ulong userId, string text, bool isTTS = false, Embed embed = null, RequestOptions options = null)
            => DMUser(_discord.GetUser(userId), text, isTTS, embed, options);
        public async Task<IUserMessage> DMUser(IUser user, string text, bool isTTS = false, Embed embed = null, RequestOptions options = null)
            => await SendAsync(await user.GetOrCreateDMChannelAsync(), user, text, isTTS, embed, options);

    }
}
