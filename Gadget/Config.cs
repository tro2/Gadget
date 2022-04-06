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
            string? filepath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Data\\botconfig.json";

            if (File.Exists(filepath))
            {
                try
                {
                    BotConfig? config = JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText(filepath));

                    if (config != null)
                    {
                        this.Token = config.Token;
                        this.OwnerId = config.OwnerId;

                        if (containsDefaultValues())
                        {
                            Console.WriteLine("Botconfig cotains default values, please overwrite them before starting the bot again");

                            return false;
                        }

                        return true;
                    }

                    //TODO
                    //Log that the config was in an unreadable format
                }
                catch (Exception e)
                {
                    //TODO
                    //Logging
                    Console.WriteLine(e.Message);
                }
            }

            BotConfig inputConfig = new BotConfig()
            {
                Token = "token",
                OwnerId = "-1"
            };

            string input = JsonConvert.SerializeObject(inputConfig);

            System.IO.File.WriteAllText(filepath, input);

            //TODO
            //Proper logging of below lines
            Console.WriteLine("New botconfig.json created with default values, overwrite them before starting the bot");

            return false;
        }

        public bool containsDefaultValues()
        {
            return (Token == "token" && OwnerId == "-1");
        }
    }
}