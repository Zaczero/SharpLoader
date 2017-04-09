﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
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
        private const string Version = "1.1";

        private const int ReadBufferSize = 255;

        private const string ConfigFileName = "SharpLoader.ini";
        private static readonly string MyPath = Process.GetCurrentProcess().MainModule.FileName;
        private static readonly string MyDirectory = Path.GetDirectoryName(MyPath);
        private static readonly string ConfigPath = Path.Combine(MyDirectory, ConfigFileName);

        public static void Main(string[] args)
        {
            Console.Title = $"SharpLoader";

            var dragDropSources = new List<string>();
            var seed = -1;

            for (var i = 0; i < args.Length; i++)
            {
                // Argument is path
                if (args[i] != MyPath && (File.Exists(args[i]) || Directory.Exists(args[i])))
                {
                    // Directory
                    if (File.GetAttributes(args[i]).HasFlag(FileAttributes.Directory))
                    {
                        var files = GetFilesFromDirectory(args[i]);
                        dragDropSources.AddRange(files);
                    }
                    // File
                    else
                    {
                        dragDropSources.Add(args[i]);
                    }
                }
                // Normal argument
                else
                {
                    // Multiple arguments
                    if (i + 1 < args.Length)
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
                    // Single argument
                    else
                    {
                        
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

            var primaryColorValue = new Random(Environment.TickCount).Next(10, 14 + 1);
            var secondaryColorValue = primaryColorValue - 8;

            Console.ForegroundColor = (ConsoleColor)primaryColorValue;
            Console.WriteLine($"-=: SharpLoader v{Version}");
            Console.ForegroundColor = (ConsoleColor)secondaryColorValue;
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

            var baseDirectoryReadSb     = new StringBuilder(ReadBufferSize);
            var assembliesReadSb        = new StringBuilder(ReadBufferSize);
            var sourceFilesReadSb       = new StringBuilder(ReadBufferSize);
            var outputNameReadSb        = new StringBuilder(ReadBufferSize);
            var autoRunReadSb           = new StringBuilder(ReadBufferSize);
            var compilerArgumentsReadSb = new StringBuilder(ReadBufferSize);

            WinApi.GetPrivateProfileString("SharpLoader", "Directory", string.Empty, baseDirectoryReadSb, ReadBufferSize, ConfigPath);
            WinApi.GetPrivateProfileString("SharpLoader", "Assemblies", string.Empty, assembliesReadSb, ReadBufferSize, ConfigPath);
            WinApi.GetPrivateProfileString("SharpLoader", "Sources", string.Empty, sourceFilesReadSb, ReadBufferSize, ConfigPath);
            WinApi.GetPrivateProfileString("SharpLoader", "Output", string.Empty, outputNameReadSb, ReadBufferSize, ConfigPath);
            WinApi.GetPrivateProfileString("SharpLoader", "AutoRun", string.Empty, autoRunReadSb, ReadBufferSize, ConfigPath);
            WinApi.GetPrivateProfileString("SharpLoader", "Arguments", string.Empty, compilerArgumentsReadSb, ReadBufferSize, ConfigPath);

            var baseDirectory     = baseDirectoryReadSb.ToString();
            var userAssemblies        = assembliesReadSb.ToString().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            string[] sourceFiles;
            var outputName        = $"{outputNameReadSb}-{DateTime.Now:dd-MM-yyyy}.exe";
            var autoRun           = autoRunReadSb.ToString().Equals("true", StringComparison.OrdinalIgnoreCase);
            var compilerArguments = compilerArgumentsReadSb.ToString();

            // Drag n Drop
            if (dragDropSources.Count != 0)
            {
                sourceFiles = dragDropSources.ToArray();
            }
            // Read from config
            else
            {
                sourceFiles = sourceFilesReadSb.ToString().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            }

            // Check values
            if (userAssemblies.Length == 0)
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
            var userSources = new string[sourceFiles.Length];
            for (var i = 0; i < userSources.Length; i++)
            {
                // Add directory to source files paths
                if (!string.IsNullOrWhiteSpace(baseDirectory))
                {
                    sourceFiles[i] = Path.Combine(baseDirectory, sourceFiles[i]);
                }

                if (!File.Exists(sourceFiles[i]))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"-=: Source file not found ({sourceFiles[i]})");

                    Console.ReadKey();

                    Environment.Exit(5);
                }

                userSources[i] = File.ReadAllText(sourceFiles[i]);
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("-=: Randomizing...");

            for (var i = 0; i < userSources.Length; i++)
            {
                randomizer.Randomize(ref userSources[i]);
            }
            for (var i = 0; i < randomizer.InjectSources.Count; i++)
            {
                var tmp = randomizer.InjectSources[i];
                randomizer.Randomize(ref tmp);
                randomizer.InjectSources[i] = tmp;
            }

            // Inject sources
            var compileSources = userSources.ToList();
            compileSources.AddRange(randomizer.InjectSources);

            // Inject bytes
            if(randomizer.InjectBytes.Count > 0)
            {
                var output = new StringBuilder("new byte[] {");
                foreach (var b in randomizer.InjectBytes)
                {
                    output.Append($"{b},");
                }
                output.Remove(output.Length - 1, 1);
                output.Append("}");

                compileSources.Add($"namespace {randomizer.InjectBytesNamespace}" +
                               $"{{" +
                               $"public static class {randomizer.InjectBytesClass}" +
                               $"{{" +
                               $"public static byte[] {randomizer.InjectBytesProperty} = {output};" +
                               $"}}" +
                               $"}}");
            }

            // Inject bools
            if (randomizer.InjectBools.Count > 0)
            {
                var output = new StringBuilder("new bool[] {");
                foreach (var b in randomizer.InjectBools)
                {
                    output.Append($"{b.ToString().ToLower()},");
                }
                output.Remove(output.Length - 1, 1);
                output.Append("}");

                compileSources.Add($"namespace {randomizer.InjectBoolsNamespace}" +
                               $"{{" +
                               $"public static class {randomizer.InjectBoolsClass}" +
                               $"{{" +
                               $"public static bool[] {randomizer.InjectBoolsProperty} = {output};" +
                               $"}}" +
                               $"}}");
            }

            // Inject ints
            if (randomizer.InjectInts.Count > 0)
            {
                var output = new StringBuilder("new int[] {");
                foreach (var b in randomizer.InjectInts)
                {
                    output.Append($"{b},");
                }
                output.Remove(output.Length - 1, 1);
                output.Append("}");

                compileSources.Add($"namespace {randomizer.InjectIntsNamespace}" +
                               $"{{" +
                               $"public static class {randomizer.InjectIntsClass}" +
                               $"{{" +
                               $"public static int[] {randomizer.InjectIntsProperty} = {output};" +
                               $"}}" +
                               $"}}");
            }

            // Inject strings
            if (randomizer.InjectStrings.Count > 0)
            {
                var output = new StringBuilder("new string[] {");
                foreach (var b in randomizer.InjectStrings)
                {
                    output.Append($"\"{b}\",");
                }
                output.Remove(output.Length - 1, 1);
                output.Append("}");

                compileSources.Add($"namespace {randomizer.InjectStringsNamespace}" +
                               $"{{" +
                               $"public static class {randomizer.InjectStringsClass}" +
                               $"{{" +
                               $"public static string[] {randomizer.InjectStringsProperty} = {output};" +
                               $"}}" +
                               $"}}");
            }

            // Inject assemblies
            var compileAssemblies = userAssemblies.ToList();
            foreach (var t in randomizer.InjectAssemblies)
            {
                if (compileAssemblies.All(a => a != t))
                {
                    compileAssemblies.Add(t);
                }
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("-=: Compiling...");

            compiler.Compile(outputName, compilerArguments, compileAssemblies.ToArray(), compileSources.ToArray());

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

        private static List<string> GetFilesFromDirectory(string directory)
        {
            var dir = new DirectoryInfo(directory);

            var files = dir.GetFiles();
            var subdirs = dir.GetDirectories();

            var returnList = files.Select(file => file.FullName).ToList();

            foreach (var subdir in subdirs)
            {
                returnList.AddRange(GetFilesFromDirectory(subdir.FullName));
            }

            return returnList;
        }
    }
}
