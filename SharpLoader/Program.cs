using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using SharpLoader.Core;

namespace SharpLoader
{
    public class Program
    {
        /* Bugs:
         */

        /* Limitations:
         * doesn't support $ strings
         * doesn't support @ strings
         * doesn't support VS code
         * doesn't support => properties
         * doesn't support few unicode values
         */

        /* Exit codes:
         * 0 - default
         * 1 - data file not found
         * 2 - incorrect data value
         * 3 - entry point not found
         * 4 - compilation error
         * 5 - source file not found
         */

        private const string Author = "Zaczero";
        private const string Version = "1.0";

        private const int ReadBufferSize = 255;

        private const string ConfigFileName = "SharpLoader.ini";
        private static readonly string ConfigPath = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), ConfigFileName);

        public static void Main(string[] args)
        {
            Console.Title = $"-=: SharpLoader v{Version}";

            var seed = -1;

            for (var i = 0; i < args.Length - 1; i++)
            {
                // Get seed from arguments
                if (args[i] == "-seed")
                {
                    var result = int.TryParse(args[i + 1], out seed);
                    if (!result)
                    {
                        throw new Exception($"invalid seed value: {args[i + 1]}");
                    }
                }
            }

            // Generate random seed
            if (seed == -1)
            {
                seed = new Random(Environment.TickCount).Next(0, int.MaxValue);
            }

            var randomizer = new SourceRandomizer(seed);
            var compiler = new RuntimeCompiler();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"-=: SharpLoader v{Version} initialized");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"-=: Created by {Author}");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"-=: Seed : {seed}");

            Console.WriteLine();

            // Data file not found
            if (!File.Exists(ConfigFileName))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"-=: Config file not found ({ConfigFileName})");

                WinApi.WritePrivateProfileString("SharpLoader", "Assemblies", "", ConfigPath);
                WinApi.WritePrivateProfileString("SharpLoader", "Directory", "", ConfigPath);
                WinApi.WritePrivateProfileString("SharpLoader", "Sources", "", ConfigPath);
                WinApi.WritePrivateProfileString("SharpLoader", "Output", "SharpLoader", ConfigPath);
                WinApi.WritePrivateProfileString("SharpLoader", "AutoRun", "false", ConfigPath);
                WinApi.WritePrivateProfileString("SharpLoader", "Arguments", "/platform:anycpu32bitpreferred", ConfigPath);

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"-=: Default config file generated");

                Console.ReadKey();

                Environment.Exit(1);
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("-=: Reading...");

            var directoryReadSb         = new StringBuilder(ReadBufferSize);
            var assembliesReadSb        = new StringBuilder(ReadBufferSize);
            var sourceFilesReadSb       = new StringBuilder(ReadBufferSize);
            var outputNameReadSb        = new StringBuilder(ReadBufferSize);
            var autoRunReadSb           = new StringBuilder(ReadBufferSize);
            var compilerArgumentsReadSb = new StringBuilder(ReadBufferSize);

            WinApi.GetPrivateProfileString("SharpLoader", "Directory", string.Empty, directoryReadSb, ReadBufferSize, ConfigPath);
            WinApi.GetPrivateProfileString("SharpLoader", "Assemblies", string.Empty, assembliesReadSb, ReadBufferSize, ConfigPath);
            WinApi.GetPrivateProfileString("SharpLoader", "Sources", string.Empty, sourceFilesReadSb, ReadBufferSize, ConfigPath);
            WinApi.GetPrivateProfileString("SharpLoader", "Output", string.Empty, outputNameReadSb, ReadBufferSize, ConfigPath);
            WinApi.GetPrivateProfileString("SharpLoader", "AutoRun", string.Empty, autoRunReadSb, ReadBufferSize, ConfigPath);
            WinApi.GetPrivateProfileString("SharpLoader", "Arguments", string.Empty, compilerArgumentsReadSb, ReadBufferSize, ConfigPath);

            var directory         = directoryReadSb.ToString();
            var assemblies        = assembliesReadSb.ToString().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            var sourceFiles       = sourceFilesReadSb.ToString().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            var outputName        = $"{outputNameReadSb}-{DateTime.Now:dd-MM-yyyy}.exe";
            var autoRun           = autoRunReadSb.ToString().Equals("true", StringComparison.OrdinalIgnoreCase);
            var compilerArguments = compilerArgumentsReadSb.ToString();

            // Check values
            if (assemblies.Length == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"-=: Assemblies are not given");

                Console.ReadKey();

                Environment.Exit(2);
            }

            if (sourceFiles.Length == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"-=: Sources are not given");

                Console.ReadKey();

                Environment.Exit(2);
            }

            if (string.IsNullOrWhiteSpace(outputName))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"-=: Output name is not given");

                Console.ReadKey();

                Environment.Exit(2);
            }

            // Read sources
            var sources = new string[sourceFiles.Length];
            for (var i = 0; i < sources.Length; i++)
            {
                // Add directory to source files paths
                if (!string.IsNullOrWhiteSpace(directory))
                {
                    sourceFiles[i] = Path.Combine(directory, sourceFiles[i]);
                }

                if (!File.Exists(sourceFiles[i]))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"-=: Source file not found ({sourceFiles[i]})");

                    Console.ReadKey();

                    Environment.Exit(5);
                }

                sources[i] = File.ReadAllText(sourceFiles[i]);
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("-=: Randomizing...");

            for (var i = 0; i < sources.Length; i++)
            {
                randomizer.Randomize(ref sources[i]);
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("-=: Compiling...");

            compiler.Compile(outputName, compilerArguments, assemblies, sources);

            if (autoRun && File.Exists(outputName))
            {
                Console.WriteLine("-=: Starting...");
                Process.Start(outputName);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"-=: DONE [{outputName}] (press any key to exit)");
            Console.ReadKey();

            Environment.Exit(0);
        }
    }
}
