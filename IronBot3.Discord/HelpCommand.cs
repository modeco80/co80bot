using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;

using IronBot3.Api;

namespace IronBot3.Discord
{

    /// <summary>
    /// The help command for IronBot3.Discord/co80bot
    /// </summary>
    public class HelpCommand : IIb3Command
    {
        string IIb3Command.Name { get { return "help"; } }
        string IIb3Command.Description { get { return "Help on all of the functions of co80bot!"; } }
        string IIb3Command.HelpDescription { get { return "(Optional: Command to look up)"; } }


        async Task IIb3Command.Run(IIb3Bot callingBot, DiscordClient client, DiscordUser user, DiscordChannel channel, string[] args)
        {
            bool ShouldShowUsage = false;
            bool Found = false;
            string ExplicitCommand = null;

            if (args.Length >= 1)
            {
                ShouldShowUsage = true;
                ExplicitCommand = args[0];
            }

            DiscordEmbedBuilder builder = new DiscordEmbedBuilder
            {
                Title = "co80bot help",
                Description = "Help on your favorite, ~~shoddily coded~~ bot",
                Color = new DiscordColor(0xFF9900)
            };


            if (ExplicitCommand != null && ShouldShowUsage)
            {
                builder.Title = $"co80bot help - lookup {ExplicitCommand}";
                // explicit command to look up
                // also prints usage for this specific command
                foreach (IIb3Command command in DiscordBot.commands)
                {
                    if (command.Name == ExplicitCommand)
                    {
                        Found = true;
                        builder.AddField(command.Name, $"Description:\n{command.Description}\nUsage:\n```\n{ ((DiscordBot)callingBot).Prefix }{ command.Name } { command.HelpDescription}\n```", true);
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }

                // If we somehow got past here and the command wasn't found: ?Que?
                if (!Found)
                    builder.AddField("Error!", "Your specified command couldn't be found!", true);
            }
            else
            {
                // small list of every command
                foreach (IIb3Command command in DiscordBot.commands)
                {
                    builder.AddField(command.Name, command.Description, true);
                }
            }

            await client.SendMessageAsync(channel, null, false, builder.Build());
        }
    }
}
