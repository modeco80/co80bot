using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Net.WebSocket;

using IronBot3.Api;


namespace IronBot3.Discord
{

    public class Ib3DiscordConfig
    {
        [JsonProperty("token")]
        public string Token { get; private set; }
        [JsonProperty("prefix")]
        public string Prefix { get; private set; }
    }


    public class DiscordBot : IIb3Bot
    {
        Ib3DiscordConfig conf;
        DiscordClient client;
        public static List<IIb3Command> commands = new List<IIb3Command>();

        public string Prefix { get; set; }

        /// <summary>
        /// Runs a command for a user, by parsing and finding the IIb3Command by name,
        /// then launching it.
        /// </summary>
        /// <param name="user">D#+ user</param>
        /// <param name="client">D#+ discord client</param>
        /// <param name="channel">D#+ discord channel</param>
        /// <param name="message">The content of the message.</param>
        /// <returns>Absoultely nothing. Enjoy.</returns>
        private async Task RunCommand(DiscordUser user, DiscordClient client, DiscordChannel channel, string message) {
            if (user == client.CurrentUser || user.IsBot) return;

            if (message.StartsWith(this.Prefix))
            {
                CommandLookup lookup = CommandLookup.ParseInputToLookup(message, this.Prefix);
                foreach (IIb3Command command in commands)
                {
                    if (command.Name == lookup.Name)
                    {
                        await command.Run(this, client, user, channel, lookup.Arguments);
                    }
                }
            }
        }

        /// <summary>
        /// Loads IronBot3 configuration from <paramref name="FilesystemPath"/>.
        /// </summary>
        /// <param name="FilesystemPath">Path on filesystem to the Ib3 configuration JSON.</param>
        /// <returns></returns>
        private async Task<int> LoadConfiguration(string FilesystemPath)
        {
            string raw_data = "";

            try
            {
                using (FileStream ConfigReader = File.OpenRead(FilesystemPath))
                {
                    StreamReader stream = new StreamReader(ConfigReader, new UTF8Encoding(false));
                    raw_data = await stream.ReadToEndAsync();
                }
            } catch
            {
                await Logger.Log($"Error loading {FilesystemPath}.", Logger.Severity.Error);
                return 1;
            }
            conf = JsonConvert.DeserializeObject<Ib3DiscordConfig>(raw_data);

            raw_data = null;
            return 0;
        }

        /// <summary>
        /// Implementation of IIb3Bot.Start for Ironbot3.Discord/co80bot
        /// </summary>
        /// <returns></returns>
        async Task IIb3Bot.Start()
        {
            #region Command initalization
            commands.Add(new HelpCommand());
            #endregion

            int LoadStatus = await LoadConfiguration("ironbot3_discord.json");

            if(LoadStatus == 1)
            {
                await Logger.Log("Could not load config, exiting", Logger.Severity.Error);
                return;
            }

            this.Prefix = conf.Prefix;

            client = new DiscordClient(new DiscordConfiguration
            {
                Token = conf.Token,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Warning
            });

            client.SetWebSocketClient<WebSocket4NetClient>();


            client.Ready += async (e) =>
            {
                await Logger.Log($"Client ready (User {e.Client.CurrentUser.Username}#{e.Client.CurrentUser.Discriminator})");
            };

            client.MessageCreated += async (e) =>
            {
                await RunCommand(e.Author, client , e.Channel, e.Message.Content);
            };

            await client.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
