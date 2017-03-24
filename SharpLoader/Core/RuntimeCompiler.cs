using System;
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

        public bool Compile(out Assembly compiledAssembly, string[] assemblies, params string[] sources)
        {
            var parameters = new CompilerParameters
            {
                TreatWarningsAsErrors = false,
                IncludeDebugInformation = false,
                GenerateInMemory = true,
                GenerateExecutable = false,
            };

            parameters.ReferencedAssemblies.AddRange(assemblies);

            var result = _compiler.CompileAssemblyFromSource(parameters, sources);
            if (result.Errors.HasErrors)
            {
                Debugger.Break();
                compiledAssembly = null;
                return false;
            }

            compiledAssembly = result.CompiledAssembly;
            return true;
        }
    }
}
