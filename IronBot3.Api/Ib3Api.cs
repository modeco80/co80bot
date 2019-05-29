using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace IronBot3.Api
{

    public interface IIb3Bot
    {
        Task Start();
    }

    public class Logger
    {

        public enum Severity
        {
            Info,
            Warning,
            Error
        };

        public static async Task Log(string message, Severity sev = Severity.Info)
        {
            switch (sev)
            {
                default:
                    break;

                case Severity.Info:
                    {
                        
                        Console.WriteLine($"[Info] {message}");
                    }
                    break;

                case Severity.Warning:
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine($"[Warning] {message}");
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    break;

                case Severity.Error:
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[Error] {message}");
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    break;

            }

        }
    }

    }
