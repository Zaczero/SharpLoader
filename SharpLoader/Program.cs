using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SharpLoader.Core;

namespace SharpLoader
{
    public class Program
    {
        // doesn't support $ strings
        // doesn't support vs code (#if etc.)

        public static void Main(string[] args)
        {
            Console.Title = "-=: SharpLoader (Yami) :=-";

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

            var extractor = new ResourceExtractor("SharpLoader");
            var randomizer = new SourceRandomizer(seed);
            var compiler = new RuntimeCompiler();

            var oldFore = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("-=: SharpLoader initialized");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("-=: Created by Zaczero");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"-=: Seed : {seed}");

            Console.WriteLine();
            
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("-=: Extracting...");

            var entrypoint = extractor.Extract("Yami.EntryPoint.cs");
            var memory = extractor.Extract("Yami.Memory.cs");
            var winapi = extractor.Extract("Yami.WinApi.cs");
            var netvars = extractor.Extract("Yami.NetVars.cs");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("-=: Randomizing...");

            randomizer.Randomize(ref entrypoint);
            randomizer.Randomize(ref memory);
            randomizer.Randomize(ref winapi);
            randomizer.Randomize(ref netvars);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("-=: Compiling...");

            Assembly compiled;
            compiler.Compile(out compiled, new [] {"System.dll", "System.Windows.Forms.dll"}, entrypoint, memory, winapi, netvars);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("-=: Starting...");

            var mod = compiled.GetModules()[0];
            var type = mod.GetType("SharpLoader.Yami.EntryPoint");
            var method = type.GetMethod("Start");

            Console.WriteLine();
            Console.ForegroundColor = oldFore;

            method.Invoke(null, null);

            Debugger.Break();
        }
    }
}
