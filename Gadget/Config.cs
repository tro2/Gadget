namespace Gadget
{
    class Config
    {
        //==========================================================================
        // Constructors

        public Config()
        {
            Token = "";
            OwnerId = "";
        }

        //==========================================================================
        // Properties

        public string Token { get; }

        public string OwnerId { get; }
    }
}