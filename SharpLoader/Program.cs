using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SharpLoader.Core;

namespace SharpLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            var randomizer = new SourceRandomizer(seed: 123);
            var compiler = new RuntimeCompiler();

            var someSauce = "using System;public class Class1{public static void Method1(){Console.WriteLine(\"Hello ;)\");[trash:15]}}";
            randomizer.Randomize(ref someSauce);

            Assembly compiled;
            var result = compiler.Compile(out compiled, new [] {"System.dll"}, someSauce);
            Debugger.Break();
        }
    }
}
