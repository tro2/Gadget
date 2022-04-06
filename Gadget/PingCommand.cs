using Discord.Commands;

namespace Gadget
{
    public class PingModule : ModuleBase<SocketCommandContext>
    {
        // ~say hello world -> hello world
        [Command("ping")]
        [Summary("Returns the bot's ping")]
        public async Task PingAsync()
        {
            await ReplyAsync("Pong");
        }
    }
}