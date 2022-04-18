using Discord.Interactions;

namespace Gadget
{
    public class SlashTestModule : InteractionModuleBase<SocketInteractionContext>
    {
        // ~say hello world -> hello world
        [SlashCommand("test", "Test of slash commands")]
        public async Task SlashTestAsync()
        {
            Thread.Sleep(2500);

            await RespondAsync("Slash command test successful");
        }
    }
}