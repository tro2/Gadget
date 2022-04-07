using Discord.Commands;

namespace Gadget
{
    public class TestMessageModule : ModuleBase<SocketCommandContext>
    {
        // ~say hello world -> hello world
        [Command("test")]
        [Summary("Test")]
        public async Task TestMessageAsync()
        {
            await ReplyAsync("Test Successful");
        }
    }
}