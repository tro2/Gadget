using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Interactions;

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
        private readonly CommandService _commands;
        private readonly InteractionService _interactions;
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
                GatewayIntents = GatewayIntents.All
            });

            _commands = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Info,
                CaseSensitiveCommands = false,
                DefaultRunMode = Discord.Commands.RunMode.Async
            });

            _interactions = new InteractionService(_client, new InteractionServiceConfig
            {
                LogLevel = LogSeverity.Info,
                DefaultRunMode = Discord.Interactions.RunMode.Async
            });

            _botConfig = new BotConfig();

            _commandHandler = new CommandHandler(_client, _commands, _interactions, _botConfig);

            _client.Log += LogAsync;
            _commands.Log += LogAsync;
            _interactions.Log += LogAsync;

            _client.Ready += ReadyAsync;
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
            if (!Database.TestPing())
            {
                return;
            }

            // Centralize the logic for commands into a separate method.
            await _commandHandler.InitializeAsync();

            // Login and connect.

            await _client.LoginAsync(TokenType.Bot, _botConfig.Token, true);
            await _client.StartAsync();

            // Wait infinitely so your bot actually stays connected.
            await Task.Delay(Timeout.Infinite);
        }

        private Task LogAsync(LogMessage message)
        {
            Logger.Write(message);

            return Task.CompletedTask;
        }

        private async Task ReadyAsync()
        {
            _commandHandler.ValidateCommandTables(); //2nd step of making sure commands are ready to go

            await _interactions.RegisterCommandsToGuildAsync(_botConfig.TestGuildId);

            var sc = _interactions.SlashCommands.Count;

            Logger.Write(LogSeverity.Info, "InteractionService", $"Successfully registered {sc} slash commands to the test guild");
        }
    }
}