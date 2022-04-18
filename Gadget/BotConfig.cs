using System.Reflection;
using Newtonsoft.Json;

namespace Gadget
{
    [JsonObject(MemberSerialization.OptIn)]
    public class BotConfig
    {
        //==========================================================================
        // Constructors

        public BotConfig()
        {
            Token = "token";
            OwnerId = ulong.MinValue;
            TestGuildId = ulong.MinValue;
            IgnoredGuilds = new ulong[] { ulong.MinValue };
        }

        //==========================================================================
        // Properties

        [JsonProperty]
        public string Token { get; set; }
        [JsonProperty]
        public ulong OwnerId { get; set; }
        [JsonProperty]
        public ulong TestGuildId { get; set; }
        [JsonProperty]
        public ulong[] IgnoredGuilds { get; set; }

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

                    if (config != null)
                    {
                        this.Token = config.Token;
                        this.OwnerId = config.OwnerId;
                        this.TestGuildId = config.TestGuildId;
                        this.IgnoredGuilds = config.IgnoredGuilds;

                        if (ContainsDefaultValues())
                        {
                            WriteConfig(filepath, config);

                            Logger.Write(Discord.LogSeverity.Error, "BotConfig", "Bot config cotains default values, overwrite them before starting the bot again");
                        }
                        else
                        {
                            success = true;
                        }
                    }
                    else
                    {
                        Logger.Write(Discord.LogSeverity.Error, "BotConfig", "Bot config was in an unreadable format");
                    }
                }
                catch (Exception e)
                {
                    Logger.Write(Discord.LogSeverity.Error, "BotConfig", "", e);
                }
            }
            else
            {
                WriteConfig(filepath, new BotConfig());

                Logger.Write(Discord.LogSeverity.Error, "BotConfig", "New botconfig.json created with default values, overwrite them before starting the bot");
            }

            return success;
        }

        private void WriteConfig(string filepath, BotConfig config)
        {
            string input = JsonConvert.SerializeObject(config);

            System.IO.File.WriteAllText(filepath, input);
        }

        private bool ContainsDefaultValues()
        {
            return (
                Token == "token" ||
                OwnerId == ulong.MinValue ||
                TestGuildId == ulong.MinValue ||
                IgnoredGuilds.Length > 0 &&
                IgnoredGuilds[0] == ulong.MinValue);
        }
    }
}