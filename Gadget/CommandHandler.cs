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
        private readonly BotConfig _botConfig;

        public CommandHandler(DiscordSocketClient client, CommandService commands, InteractionService interactions, BotConfig botConfig)
        {
            _commands = commands;
            _client = client;
            _interactions = interactions;
            _botConfig = botConfig;
        }

        public async Task InitializeAsync()
        {
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

            // Hook the MessageReceived event into our command handler
            _client.MessageReceived += HandleCommandAsync;
            _client.SlashCommandExecuted += HandleSlashCommandAsync;

            //Database command tables are validated upon readyevent
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

            if (!context.IsPrivate) //command executed from a guild
            {
                foreach (var id in _botConfig.IgnoredGuilds)
                {
                    if (context.Guild.Id == id)
                        return;
                }

                Database.Retrieve("Config", "prefix", Convert.ToString(context.Guild.Id), out string prefix);

                if (!message.HasStringPrefix(prefix, ref argPos))
                {
                    return;
                }
            }
            else //command executed from a private channel or group
            {
                //TODO
                //Allow command usage with any prefix from any guild, or no prefix at all

                var result = _commands.Search(message.Content);

                if (!result.IsSuccess)
                {
                    if (message.Content.ToLower().Contains("modmail")) //Hardcoding a modmail response so people don't get confused
                    {
                        await context.Channel.SendMessageAsync("This bot's modmail isn't currently enabled.");
                    }

                    return;
                }
            }


            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            var command = await _commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: null);

            if (!command.IsSuccess)
            {
                await context.Channel.SendMessageAsync(command.ErrorReason);
            }
        }

        private async Task HandleSlashCommandAsync(SocketSlashCommand command)
        {
            var context = new SocketInteractionContext(_client, command);
            await _interactions.ExecuteCommandAsync(context, null);
        }

        public bool ValidateCommandTables()
        {
            bool validated = Database.ValidateCommands(_client.Guilds, _commands.Commands);

            if (validated)
            {
                Logger.Write(Discord.LogSeverity.Info, "CommandHandler", "Successfully validated commands");
            }
            else
            {
                Logger.Write(Discord.LogSeverity.Error, "CommandHandler", "Failed to validate commands");
            }

            return validated;
        }
    }
}