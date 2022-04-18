using Discord;
using Discord.Commands;

namespace Gadget
{
    public class TestMessageModule : ModuleBase<SocketCommandContext>
    {
        // ~say hello world -> hello world
        [Command("test")]
        [Summary("Test")]
        [RequireRole()]
        public async Task TestMessageAsync()
        {
            var heartEmoji = new Emoji("\U0001f495");

            await Context.Message.AddReactionAsync(heartEmoji);
        }
    }
}