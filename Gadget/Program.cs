using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace Gadget
{
    class Program
    {
        public static Task Main(string[] args) => new Program().MainAsync();

        private DiscordSocketClient _client;

        public async Task MainAsync()
        {
            // var socketConfig = new DiscordSocketConfig()
            // {
            //     GatewayIntents = GatewayIntents.All
            // };

            _client = new DiscordSocketClient(/*socketConfig*/);

            _client.Log += Log;
            _client.MessageReceived += CommandManager.CommandHandler;

            // Some alternative options would be to keep your token in an Environment Variable or a standalone file.
            // var token = Environment.GetEnvironmentVariable("NameOfYourEnvironmentVariable");
            // var token = File.ReadAllText("token.txt");
            // var token = JsonConvert.DeserializeObject<AConfigurationClass>(File.ReadAllText("config.json")).Token;
            var token = "ODM4OTg2NjIzMTkwMTcxNjg5.YJDFLg.l-sSbpmtOKiXGdeHubGZ13I9-8A";

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
