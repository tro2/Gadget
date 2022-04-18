using Discord.Commands;
using Discord;

namespace Gadget
{
    class RequireRoleAttribute : PreconditionAttribute
    {
        public RequireRoleAttribute()
        {

        }

#pragma warning disable CS1998 //The task needs to be async, but the return result isn't
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var guildUser = context.User as IGuildUser;
            if (guildUser == null)
                return PreconditionResult.FromError("This command cannot be executed in dm's.");

            string commandName = command.Name;

            //Database.Retrieve("CommandData", commandName, Convert.ToString(context.Guild.Id), out string output);

            return PreconditionResult.FromSuccess();
        }
    }
}