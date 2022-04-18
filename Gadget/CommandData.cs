namespace Gadget
{
    class CommandData
    {
        ulong MinRoleId;
        ulong[] AllowedRoleIds;

        public CommandData()
        {
            MinRoleId = ulong.MinValue;
            AllowedRoleIds = new ulong[] { ulong.MinValue };
        }

        public void WarningRemover()
        {
            ulong num = MinRoleId + AllowedRoleIds[0];
        }
    }
}