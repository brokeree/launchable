using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Launchable.Helpers;
using Launchable.Runners;

namespace Launchable
{
    public class LaunchableRunner
    {
        static LaunchableRunner()
        {
            DirectoryHelpers.SetCurrentDirectory();
        }

        public static void Run<T>(string[] args, bool throwOnError = false) where T : ILaunchable, new()
        {
            try
            {
                RunImpl<T>(args);
            }
            catch (Exception ex)
            {
                Log.StartupError(ex.ToString());
                if (throwOnError)
                {
                    throw;
                }
            }
        }

        private static void RunImpl<T>(string[] args) where T : ILaunchable, new()
        {
            args = args ?? new string[0];

            if (!args.Any())
            {
                ConsoleRunner.Run<T>();
                return;
            }

            switch (args[0].ToLower())
            {
                case "--service":
                    ServiceRunner.Run<T>();
                    break;
                case "--console":
                    ConsoleRunner.Run<T>();
                    break;

                case "--install":
                case "-i":
                    ServiceActions.Install(args.Length > 1 ? args[1] : null);
                    break;
                case "--uninstall":
                case "-u":
                    ServiceActions.Uninstall();
                    break;
                case "--start":
                    ServiceActions.Start();
                    break;
                case "--stop":
                    ServiceActions.Stop();
                    break;

                default:
                    ShowUsage();
                    break;
            }
        }

        private static void ShowUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine($"\t{Path.GetFileName(Assembly.GetEntryAssembly().Location)} [<command>]");
            Console.WriteLine("To launch in console, do not specify any arguments");
            Console.WriteLine("Commands:");
            Console.WriteLine("\t-c, --console".PadRight(36) + "Run in console (with passed args)");
            Console.WriteLine("\t-i, --install [serviceName]".PadRight(36) + "Installs the service [with a custom service name]");
            Console.WriteLine("\t-u, --uninstall [serviceName]".PadRight(36) + "Uninstalls the service [with a custom service name]");
            Console.WriteLine("\t--start".PadRight(36) + "Starts the service");
            Console.WriteLine("\t--stop".PadRight(36) + "Stops the service");
        }
    }
}