using Discord.Interactions;

namespace Gadget
{
    public class SlashTestModule : InteractionModuleBase<SocketInteractionContext>
    {
        // ~say hello world -> hello world
        [SlashCommand("test", "Test of slash commands")]
        public async Task SlashTestAsync()
        {
            await RespondAsync("Slash command test successful");
        }
    }
}