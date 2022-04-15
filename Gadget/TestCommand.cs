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
            Thread.Sleep(2500);

            await ReplyAsync("Test Successful");
        }
    }
}