using System.Reflection;
using Newtonsoft.Json;

namespace Gadget
{
    [JsonObject(MemberSerialization.OptIn)]
    class BotConfig
    {
        //==========================================================================
        // Constructors

        public BotConfig()
        {
            Token = "";
            OwnerId = "";
        }

        //==========================================================================
        // Properties

        [JsonProperty]
        public string Token { get; set; }
        [JsonProperty]
        public string OwnerId { get; set; }

        //==========================================================================
        // Methods

        public bool InitValues()
        {
            string? filepath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "", @"Data\botconfig.json");
            bool success = false;

            if (File.Exists(filepath))
            {
                try
                {
                    BotConfig? config = JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText(filepath));

                    if (config != null && !string.IsNullOrWhiteSpace(config.Token) && !string.IsNullOrWhiteSpace(config.OwnerId))
                    {
                        this.Token = config.Token;
                        this.OwnerId = config.OwnerId;

                        Console.WriteLine(config.Token, config.OwnerId);

                        if (ContainsDefaultValues())
                        {
                            Logger.Write(Discord.LogSeverity.Error, "Bot config cotains default values, please overwrite them before starting the bot again");
                        }
                        else
                        {
                            success = true;
                        }
                    }
                    else
                    {
                        Logger.Write(Discord.LogSeverity.Error, "Bot config was in an unreadable format");
                    }
                }
                catch (Exception e)
                {
                    Logger.Write(Discord.LogSeverity.Error, "", e);
                }
            }
            else
            {
                WriteDefaultConfig(filepath);
            }

            return success;
        }

        private void WriteDefaultConfig(string filepath)
        {
            BotConfig inputConfig = new BotConfig()
            {
                Token = "token",
                OwnerId = "-1"
            };

            string input = JsonConvert.SerializeObject(inputConfig);

            System.IO.File.WriteAllText(filepath, input);

            Logger.Write(Discord.LogSeverity.Error, "New botconfig.json created with default values, overwrite them before starting the bot");
        }

        private bool ContainsDefaultValues()
        {
            return (Token == "token" && OwnerId == "-1");
        }
    }
}