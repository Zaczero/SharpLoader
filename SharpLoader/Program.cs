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
            var extractor = new ResourceExtractor("SharpLoader");
            var randomizer = new SourceRandomizer(Environment.TickCount);
            var compiler = new RuntimeCompiler();

            var sauce = extractor.Extract("Debug.cs");
            randomizer.Randomize(ref sauce);

            Assembly compiled;
            var result = compiler.Compile(out compiled, new [] {"System.dll"}, sauce);

            var mod = compiled.GetModules()[0];
            var type = mod.GetType("SharpLoader.Debug");
            var method = type.GetMethod("WriteLine");
            method.Invoke(null, new object[] {"Welcome Random!"});

            Debugger.Break();
        }
    }
}
