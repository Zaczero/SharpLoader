using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SharpLoader.Core;

namespace SharpLoader
{
    public class Program
    {
        /* Bugs:
         */

        /* Limitations:
         * doesn't support $ strings
         * doesn't support VS code
         */

        /* Exit codes:
         * 0 - default
         * 1 - data file not found
         * 2 - incorrect data value
         * 3 - entry point not found
         * 4 - compilation error
         */

        //todo zip
        //todo remove unused references

        private const string Author = "Zaczero";
        private const string Version = "1.0";

        private const int ReadBufferSize = 255;

        private const string DataFileName = "SharpLoader.ini";
        private static readonly string DataPath = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), DataFileName);

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
                        throw new Exception("incorrect seed");
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

            var oldForegroundColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"-=: SharpLoader v{Version} initialized");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"-=: Created by {Author}");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"-=: Seed : {seed}");

            Console.WriteLine();

            // Data file not found
            if (!File.Exists(DataFileName))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"-=: Data file not found ({DataFileName})");

                WinApi.WritePrivateProfileString("SharpLoader", "Assemblies", "", DataPath);
                WinApi.WritePrivateProfileString("SharpLoader", "Sources", "", DataPath);

                WinApi.WritePrivateProfileString("EntryPoint", "Namespace", "", DataPath);
                WinApi.WritePrivateProfileString("EntryPoint", "Class", "", DataPath);
                WinApi.WritePrivateProfileString("EntryPoint", "Method", "", DataPath);
                WinApi.WritePrivateProfileString("EntryPoint", "Arguments", "", DataPath);

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"-=: Default data file generated");

                Console.ReadKey();

                Debugger.Break();
                Environment.Exit(1);
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("-=: Reading...");

            var assembliesReadSb  = new StringBuilder(ReadBufferSize);
            var sourceFilesReadSb = new StringBuilder(ReadBufferSize);
            var namespaceReadSb   = new StringBuilder(ReadBufferSize);
            var classReadSb       = new StringBuilder(ReadBufferSize);
            var methodReadSb      = new StringBuilder(ReadBufferSize);
            var argumentsReadSb   = new StringBuilder(ReadBufferSize);

            WinApi.GetPrivateProfileString("SharpLoader", "Assemblies", string.Empty, assembliesReadSb, ReadBufferSize, DataPath);
            WinApi.GetPrivateProfileString("SharpLoader", "Sources", string.Empty, sourceFilesReadSb, ReadBufferSize, DataPath);
            WinApi.GetPrivateProfileString("EntryPoint", "Namespace", string.Empty, namespaceReadSb, ReadBufferSize, DataPath);
            WinApi.GetPrivateProfileString("EntryPoint", "Class", string.Empty, classReadSb, ReadBufferSize, DataPath);
            WinApi.GetPrivateProfileString("EntryPoint", "Method", string.Empty, methodReadSb, ReadBufferSize, DataPath);
            WinApi.GetPrivateProfileString("EntryPoint", "Arguments", string.Empty, argumentsReadSb, ReadBufferSize, DataPath);

            var assemblies        = assembliesReadSb.ToString().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            var sourceFiles       = sourceFilesReadSb.ToString().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            var @namespace        = namespaceReadSb.ToString();
            var @class            = classReadSb.ToString();
            var method            = methodReadSb.ToString();
            var arguments         = argumentsReadSb.ToString().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            if (arguments.Length == 0)
            {
                arguments = null;
            }

            // Check values
            if (assemblies.Length == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"-=: Assemblies are not given");

                Console.ReadKey();

                Debugger.Break();
                Environment.Exit(2);
            }

            if (sourceFiles.Length == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"-=: Sources are not given");

                Console.ReadKey();

                Debugger.Break();
                Environment.Exit(2);
            }

            if (string.IsNullOrWhiteSpace(@namespace))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"-=: Namespace is not given");

                Console.ReadKey();

                Debugger.Break();
                Environment.Exit(2);
            }

            if (string.IsNullOrWhiteSpace(@class))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"-=: Class is not given");

                Console.ReadKey();

                Debugger.Break();
                Environment.Exit(2);
            }

            if (string.IsNullOrWhiteSpace(method))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"-=: Method is not given");

                Console.ReadKey();

                Debugger.Break();
                Environment.Exit(2);
            }

            // Read sources
            var sources = new string[sourceFiles.Length];
            for (var i = 0; i < sources.Length; i++)
            {
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

            Assembly compiled;
            compiler.Compile(out compiled, assemblies, sources);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("-=: Starting...");

            var cMod = compiled.GetModules()[0];
            if (cMod == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"-=: Module is not compiled");

                Console.ReadKey();

                Debugger.Break();
                Environment.Exit(3);
            }

            var cType = cMod.GetType($"{@namespace}.{@class}");
            if (cType == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"-=: Type {@namespace}.{@class} not found");

                Console.ReadKey();

                Debugger.Break();
                Environment.Exit(3);
            }

            var cMethod = cType.GetMethod(method);
            if (cMethod == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"-=: Method {method} not found");

                Console.ReadKey();

                Debugger.Break();
                Environment.Exit(3);
            }


            Console.WriteLine();
            Console.ForegroundColor = oldForegroundColor;

            // Object class
            try
            {
                var classInstance = Activator.CreateInstance(cType, null);
                cMethod.Invoke(classInstance, arguments);
            }
            // Static class
            catch (MissingMethodException)
            {
                cMethod.Invoke(null, arguments);
            }

            Debugger.Break();
            Environment.Exit(0);
        }
    }
}