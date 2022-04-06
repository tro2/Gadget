using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace Gadget
{
    class Program
    {
        // Program entry point
        static Task Main(string[] args)
        {
            // Call the Program constructor, followed by the 
            // MainAsync method and wait until it finishes (which should be never).
            return new Program().MainAsync();
        }

        private readonly DiscordSocketClient _client;

        // Keep the CommandService and DI container around for use with commands.
        // These two types require you install the Discord.Net.Commands package.
        private readonly CommandService _commands;
        private readonly CommandHandler _commandHandler;
        private readonly BotConfig _botConfig;

        private Program()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,

                // If you or another service needs to do anything with messages
                // (eg. checking Reactions, checking the content of edited/deleted messages),
                // you must set the MessageCacheSize. You may adjust the number as needed.
                MessageCacheSize = 200,
            });

            _commands = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Info,
                CaseSensitiveCommands = false,
            });

            _commandHandler = new CommandHandler(_client, _commands);
            _botConfig = new BotConfig();

            // Subscribe the logging handler to both the client and the CommandService.
            _client.Log += Log;
            _commands.Log += Log;

        }

        private static Task Log(LogMessage message)
        {
            Logger.Write(message);

            return Task.CompletedTask;
        }

        private async Task MainAsync()
        {
            //Preconditions
            if (!_botConfig.InitValues())
            {
                return;
            }

            //TODO
            //Setup Database

            // Centralize the logic for commands into a separate method.
            await _commandHandler.InstallCommandsAsync();

            // Login and connect.

            await _client.LoginAsync(TokenType.Bot, _botConfig.Token, true);
            await _client.StartAsync();

            // Wait infinitely so your bot actually stays connected.
            await Task.Delay(Timeout.Infinite);
        }
    }
}