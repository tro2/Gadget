using System.Reflection;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Interactions;

namespace Gadget
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly InteractionService _interactions;

        public CommandHandler(DiscordSocketClient client, CommandService commands, InteractionService interactions)
        {
            _commands = commands;
            _client = client;
            _interactions = interactions;
        }

        public async Task InitializeAsync()
        {
            // Hook the MessageReceived event into our command handler
            _client.MessageReceived += HandleCommandAsync;
            _client.SlashCommandExecuted += HandleSlashCommandAsync;

            // Here we discover all of the command modules in the entry 
            // assembly and load them. Starting from Discord.NET 2.0, a
            // service provider is required to be passed into the
            // module registration method to inject the 
            // required dependencies.
            //
            // If you do not use Dependency Injection, pass null.
            // See Dependency Injection guide for more information.
            await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
                                            services: null);

            await _interactions.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
                                            services: null);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            var message = messageParam as SocketUserMessage;
            if (message == null)
            {
                return;
            }

            if (message.Author.IsBot)
            {
                return;
            }

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(_client, message);

            int argPos = 0;
            Database.Retrieve("Config", "prefix", Convert.ToString(context.Guild.Id), out string prefix);

            // Determine if the message is a command based on the prefix
            if (!(message.HasStringPrefix(prefix, ref argPos) ||
                message.Author.IsBot))
            {
                return;
            }

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            await _commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: null);
        }

        private async Task HandleSlashCommandAsync(SocketSlashCommand command)
        {
            var context = new SocketInteractionContext(_client, command);
            await _interactions.ExecuteCommandAsync(context, null);
        }
    }
}