using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using SharpLoader.Core;

namespace SharpLoader
{
    public static class Program
    {
        /* Limitations:
         * supports only c# 5.0
         */

        /* Exit codes:
         * 0 - default
         * 1 - data file not found
         * 2 - incorrect data value
         * 3 - entry point not found
         * 4 - compilation error
         * 5 - source file not found
         * 6 - file in use
         */

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;

        public const string Author = "Zaczero";
        public const string Version = "2.0";

        private const int ReadBufferSize = 255;

        private const string ConfigFileName = "SharpLoader.ini";
        private static readonly string MyPath = Process.GetCurrentProcess().MainModule.FileName;
        private static readonly string MyDirectory = Path.GetDirectoryName(MyPath);
        private static readonly string ConfigPath = Path.Combine(MyDirectory, ConfigFileName);

        public static int Seed = -1;
        public static string Hash;

        private static MainForm _form;
        private static List<string> _dragDropPaths;
        private static bool _outToConsole;

        [STAThread]
        public static void Main(string[] args)
        {
            Console.Title = $"SharpLoader";

            // Hide cmd
            ShowWindow(GetConsoleWindow(), SW_HIDE);

            _dragDropPaths = new List<string>();

            var cmdMode = false;

            for (var i = 0; i < args.Length; i++)
            {
                // Argument is path
                if (args[i] != MyPath && (File.Exists(args[i]) || Directory.Exists(args[i])))
                {
                    // Directory
                    if (File.GetAttributes(args[i]).HasFlag(FileAttributes.Directory))
                    {
                        _dragDropPaths.AddRange(GetFilesFromDirectory(args[i]));
                    }
                    // File
                    else
                    {
                        _dragDropPaths.Add(args[i]);
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
                            var result = int.TryParse(args[i + 1], out Seed);
                            if (!result)
                            {
                                throw new Exception($"invalid seed value: {args[i + 1]}");
                            }
                        }
                    }
                    // Single argument
                    else
                    {
                        if (args[i] == "-cmd")
                        {
                            cmdMode = true;
                        }
                    }
                }
            }

            // Generate random seed
            if (Seed == -1)
            {
                Seed = new Random(Environment.TickCount).Next(0, int.MaxValue);
            }

            if (!cmdMode)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(_form = new MainForm());

                return;
            }

            // Show cmd
            ShowWindow(GetConsoleWindow(), SW_SHOW);
            _outToConsole = true;

            var compileResult = Compile();
            if (compileResult != 0)
            {
                Console.ReadKey();
            }
            Environment.Exit(compileResult);
        }

        public static int Compile()
        {
            var randomizer = new SourceRandomizer(Seed);
            var compiler = new RuntimeCompiler();

            var primaryColorValue = new Random(Environment.TickCount).Next(10, 14 + 1);
            var secondaryColorValue = primaryColorValue - 8;

            Console.ForegroundColor = (ConsoleColor)primaryColorValue;
            Out($"-=: SharpLoader v{Version}");
            Console.ForegroundColor = (ConsoleColor)secondaryColorValue;
            Out($"-=: Created by {Author}");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Out($"-=: Seed : {Seed}");

            Out("");

            // Data file not found
            if (!File.Exists(ConfigFileName))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Out($"-=: Config file not found ({ConfigFileName})");

                WinApi.WritePrivateProfileString("SharpLoader", "References", "", ConfigPath);
                WinApi.WritePrivateProfileString("SharpLoader", "Directory", "", ConfigPath);
                WinApi.WritePrivateProfileString("SharpLoader", "Sources", "", ConfigPath);
                WinApi.WritePrivateProfileString("SharpLoader", "Output", "SharpLoader", ConfigPath);
                WinApi.WritePrivateProfileString("SharpLoader", "Arguments", "/platform:anycpu32bitpreferred", ConfigPath);
                WinApi.WritePrivateProfileString("SharpLoader", "AutoRun", "false", ConfigPath);

                Console.ForegroundColor = ConsoleColor.Red;
                Out($"-=: Default config file generated");

                return 1;
            }

            Console.ForegroundColor = ConsoleColor.White;
            Out("-=: Reading...");

            var userReferencesReadSb = new StringBuilder(ReadBufferSize);
            var baseDirectoryReadSb = new StringBuilder(ReadBufferSize);
            var sourceFilesReadSb = new StringBuilder(ReadBufferSize);
            var outputNameReadSb = new StringBuilder(ReadBufferSize);
            var compilerArgumentsReadSb = new StringBuilder(ReadBufferSize);
            var autoRunReadSb = new StringBuilder(ReadBufferSize);

            WinApi.GetPrivateProfileString("SharpLoader", "References", string.Empty, userReferencesReadSb, ReadBufferSize, ConfigPath);
            WinApi.GetPrivateProfileString("SharpLoader", "Directory", string.Empty, baseDirectoryReadSb, ReadBufferSize, ConfigPath);
            WinApi.GetPrivateProfileString("SharpLoader", "Sources", string.Empty, sourceFilesReadSb, ReadBufferSize, ConfigPath);
            WinApi.GetPrivateProfileString("SharpLoader", "Output", string.Empty, outputNameReadSb, ReadBufferSize, ConfigPath);
            WinApi.GetPrivateProfileString("SharpLoader", "Arguments", string.Empty, compilerArgumentsReadSb, ReadBufferSize, ConfigPath);
            WinApi.GetPrivateProfileString("SharpLoader", "AutoRun", string.Empty, autoRunReadSb, ReadBufferSize, ConfigPath);

            var userReferences = userReferencesReadSb.ToString().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            var baseDirectory = baseDirectoryReadSb.ToString();
            string[] userSourcePaths;
            var outputName = $"{outputNameReadSb}-{DateTime.Now:dd-MM-yyyy}.exe";
            var compilerArguments = compilerArgumentsReadSb.ToString();
            var autoRun = autoRunReadSb.ToString().Equals("true", StringComparison.OrdinalIgnoreCase);

            // Drag n Drop
            if (_dragDropPaths.Count != 0)
            {
                userSourcePaths = _dragDropPaths.ToArray();
            }
            // Read from config
            else
            {
                userSourcePaths = sourceFilesReadSb.ToString().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            }

            // Check values
            if (userReferences.Length == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Out($"-=: References are not given");

                return 2;
            }

            if (string.IsNullOrWhiteSpace(outputName))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Out($"-=: Output name is not given");

                return 2;
            }

            // Read sources
            var userSourceFiles = new List<string>();
            // Add from config
            foreach (var path in userSourcePaths)
            {
                if (!File.Exists(path))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Out($"-=: Source file not found ({path})");

                    return 5;
                }

                if (path.EndsWith(".cs"))
                {
                    userSourceFiles.Add(File.ReadAllText(path));
                }
            }
            // Add from directory
            if (!string.IsNullOrWhiteSpace(baseDirectory))
            {
                userSourceFiles.AddRange(from path in GetFilesFromDirectory(baseDirectory) where path.EndsWith(".cs") select File.ReadAllText(path));
            }

            if (userSourceFiles.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Out($"-=: Source files not found");

                return 2;
            }

            Console.ForegroundColor = ConsoleColor.White;
            Out("-=: Randomizing...");

            // Randomize
            for (var i = 0; i < userSourceFiles.Count; i++)
            {
                var tmp = userSourceFiles[i];
                randomizer.Randomize(ref tmp);
                userSourceFiles[i] = tmp;
            }
            for (var i = 0; i < randomizer.InjectSources.Count; i++)
            {
                var tmp = randomizer.InjectSources[i];
                randomizer.Randomize(ref tmp);
                randomizer.InjectSources[i] = tmp;
            }

            // Inject sources
            var compileSourceFiles = userSourceFiles.ToList();
            compileSourceFiles.AddRange(randomizer.InjectSources);

            // Inject bytes
            if (randomizer.InjectBytes.Count > 0)
            {
                var output = new StringBuilder("new byte[] {");
                foreach (var b in randomizer.InjectBytes)
                {
                    output.Append($"{b},");
                }
                output.Remove(output.Length - 1, 1);
                output.Append("}");

                compileSourceFiles.Add($"namespace {randomizer.InjectBytesNamespace}" +
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

                compileSourceFiles.Add($"namespace {randomizer.InjectBoolsNamespace}" +
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

                compileSourceFiles.Add($"namespace {randomizer.InjectIntsNamespace}" +
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

                compileSourceFiles.Add($"namespace {randomizer.InjectStringsNamespace}" +
                                       $"{{" +
                                       $"public static class {randomizer.InjectStringsClass}" +
                                       $"{{" +
                                       $"public static string[] {randomizer.InjectStringsProperty} = {output};" +
                                       $"}}" +
                                       $"}}");
            }

            // Inject references
            var compileReferences = userReferences.ToList();
            foreach (var t in randomizer.InjectReferences)
            {
                if (compileReferences.All(a => a != t))
                {
                    compileReferences.Add(t);
                }
            }

            Console.ForegroundColor = ConsoleColor.White;
            Out("-=: Compiling...");

            if (File.Exists(outputName))
            {
                try
                {
                    File.Delete(outputName);
                }
                catch (UnauthorizedAccessException)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Out($"-=: File in use");

                    return 6;
                }
            }

            compiler.Compile(outputName, compilerArguments, compileReferences.ToArray(), compileSourceFiles.ToArray());

            if (autoRun && File.Exists(outputName))
            {
                Out("-=: Starting...");
                Process.Start(outputName);
            }

            var sourceBytes = new List<byte>();
            foreach (var s in compileSourceFiles)
            {
                sourceBytes.AddRange(Encoding.Default.GetBytes(s));
            }
            var sourceHash = MD5.Create().ComputeHash(sourceBytes.ToArray());

            Console.ForegroundColor = ConsoleColor.Yellow;
            Out($"-=: HASH : {Hash = ByteArrayToString(sourceHash)}");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"-=: DONE [{outputName}] (press any key to exit)");

            return 0;
        }

        private static void Out(string text)
        {
            if (_outToConsole)
            {
                Console.WriteLine(text);
            }
            else
            {
                _form.OutputText.Text += $"{text}{Environment.NewLine}";
            }
        }

        public static IEnumerable<string> GetFilesFromDirectory(string directory)
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

        public static string ByteArrayToString(IEnumerable<byte> bytes)
        {
            var returnSb = new StringBuilder();

            foreach (var b in bytes)
            {
                var hex = b.ToString("X");
                returnSb.Append(hex.Length < 2 ? '0' + hex : hex);
            }

            return returnSb.ToString();
        }
    }
}
