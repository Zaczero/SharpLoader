using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpLoader
{
    public static class Debug
    {
        public static void WriteLine(object obj)
        {
#if DEBUG
            Console.WriteLine($"[DEBUG] : {obj}");
#endif
        }
    }
}
