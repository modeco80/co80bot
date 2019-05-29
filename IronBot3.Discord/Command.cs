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
    /// Command parser class.
    /// </summary>
    class CommandLookup
    {
        public string Name { get; set; }
        public string[] Arguments;

#if false
        public override string ToString()
        {
            string s = "";
            s += $"Name: {this.Name}, Arguments:";
            foreach (string arg in this.Arguments)
            {
                s += $"{arg},";
            }
            return s;
        }
#endif
        /// <summary>
        /// Parses <paramref name="input"/> to create a new command lookup.
        /// </summary>
        /// <param name="input">The input string to parse</param>
        /// <param name="prefix"> Do I really need to explain this to you</param>
        /// <returns>A new CommandLookup filled with data.</returns>
        public static CommandLookup ParseInputToLookup(string input, string prefix)
        {
            string[] split = input.Split(' ');
            List<string> arguments = new List<string>();
            string cmd = split[0].Replace(prefix, "");

            for (int i = 1; i < split.Length; i++)
                arguments.Add(split[i]);

            return new CommandLookup
            {
                Name = cmd,
                Arguments = arguments.ToArray()
            };
        }
    }

    
    /// <summary>
    /// Command driver.
    /// </summary>
    public interface IIb3Command
    {
        string Name { get; }
        string Description { get; }
        string HelpDescription { get; } // E.g: [Param1] [Param2]...

        /// <summary>
        /// Runs this command.
        /// </summary>
        /// <param name="callingBot">The bot calling us.</param>
        /// <param name="client">The Discord client calling us.</param>
        /// <param name="user">The user who initated this command.</param>
        /// <param name="channel">The channel the command is to respond in.</param>
        /// <param name="args">Arguments for this command</param>
        /// <returns></returns>
        Task Run(IIb3Bot callingBot, DiscordClient client, DiscordUser user, DiscordChannel channel, string[] args);
    }
}
