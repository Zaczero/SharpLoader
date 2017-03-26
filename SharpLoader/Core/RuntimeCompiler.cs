﻿using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CSharp;

namespace SharpLoader.Core
{
    public class RuntimeCompiler
    {
        private readonly CSharpCodeProvider _compiler;

        public RuntimeCompiler()
        {
            _compiler = new CSharpCodeProvider();
        }

        public bool Compile(string outputName, string compilerArguments, string[] assemblies, params string[] sources)
        {
            var parameters = new CompilerParameters
            {
                TreatWarningsAsErrors = false,
                IncludeDebugInformation = false,
                GenerateInMemory = false,
                GenerateExecutable = true,
                OutputAssembly = outputName,
                CompilerOptions = compilerArguments,
            };

            parameters.ReferencedAssemblies.AddRange(assemblies);

            var result = _compiler.CompileAssemblyFromSource(parameters, sources);
            if (result.Errors.HasErrors)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"-=: Compilation error");

                Debugger.Break();
                Console.ReadKey();

                Debugger.Break();
                Environment.Exit(4);
            }

            return true;
        }
    }
}