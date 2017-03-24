using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharpLoader.Core
{
    public class ResourceExtractor
    {
        private readonly Assembly _assembly = Assembly.GetExecutingAssembly();
        private readonly string _namespace;

        public ResourceExtractor()
        {
            _namespace = string.Empty;
        }

        public ResourceExtractor(string namespaceName)
        {
            _namespace = namespaceName;
        }

        public string Extract(string resourceName)
        {
            var stream = _assembly.GetManifestResourceStream(_namespace + '.' + resourceName);
            if (stream == null)
            {
                throw new Exception("embedded resource not found");
            }
            var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }
    }
}
