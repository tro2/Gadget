using Discord;
using Discord.WebSocket;

namespace Gadget
{
    class CommandManager
    {
        //=====================================
        // Fields
        public static List<string> CommandList { get; set; }

        public static Task CommandHandler(SocketMessage message)
        {
            if (message.Author.IsBot)
                return Task.CompletedTask;

            //TODO
            //Get command data (prefix and minRole)
            string prefix = ".";
            string minRoleId = "-1";

            if (!message.Content.StartsWith(prefix))
                return Task.CompletedTask;

            string[] splitMessage = message.Content.Split(" ");
            string alias = splitMessage[0].ToLower().Replace(prefix, "");
            string[] args = splitMessage.Skip(1).ToArray();



            return Task.CompletedTask;
        }


    }
}