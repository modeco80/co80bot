using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using IronBot3.Api;

namespace IronBot3.ApiRunner
{
    class Program
    {

        const string DEFAULT_IB3_BOT = "IronBot3.Discord.dll";
        static string BotAssemblyPath = "";
        static async Task<int> Main(string[] args)
        {
            Console.WriteLine("IronBot3 API Runner - The magic that powers IronBot 3/co80bot!");
            Console.WriteLine("(C) 2019 Modeco80 <3");

            if (args.Length == 1)
            {
                if(args[0].ToLower() == "--help")
                {
                    Console.WriteLine("API Runner Usage: " + AppDomain.CurrentDomain.FriendlyName + " [Bot dll name]");
                    Console.WriteLine($"If no arguments are passed, the API Runner will use {Program.DEFAULT_IB3_BOT}.");
                    return 0;
                }

               

                // User defined bot assembly
                await Logger.Log($"Using user-defined assembly \"{args[0]}\".");
                BotAssemblyPath = args[0];
            } else
            {
                // Use the default co80bot
                await Logger.Log($"Using assembly \"{Program.DEFAULT_IB3_BOT}\".");
                BotAssemblyPath = Program.DEFAULT_IB3_BOT;
            }

            // Initalize bot
            IIb3Bot bot = LoadBot(BotAssemblyPath);
            if(bot == null)
            {
                await Logger.Log("Error loading bot, exiting.", Logger.Severity.Warning);
                return 1;
            }

            await Logger.Log($"Starting bot \"{BotAssemblyPath}\".");
            await bot.Start();

            return 0;
        }

        public static IIb3Bot LoadBot(string BotAssemPath)
        {
            Assembly BotAssembly;
            IIb3Bot bot = null;

            try
            {
                BotAssembly = Assembly.LoadFrom(BotAssemPath);
            }
            catch (Exception ex)
            {
                Logger.Log($"Error loading bot {BotAssemPath}", Logger.Severity.Error);
                Logger.Log($"Exception message: {ex.Message}", Logger.Severity.Error);
                return null;
            }

            foreach (Type AssemblyType in BotAssembly.GetTypes())
            {
                if (typeof(IIb3Bot).GetTypeInfo().IsAssignableFrom(AssemblyType.GetTypeInfo()))
                {
                    object BotInstance = Activator.CreateInstance(AssemblyType);
                    try
                    {
                        bot = (IIb3Bot)BotInstance;
                        Logger.Log($"Bot assembly \"{BotAssemPath}\" loaded successfully.");
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Error loading bot {BotAssemPath}", Logger.Severity.Error);
                        Logger.Log($"Exception message: {ex.Message}", Logger.Severity.Error);
                        return null;
                    }
                    return bot;
                }
            }

            // Return a nullref if there is an error.
            return null;
        }
    }
}
