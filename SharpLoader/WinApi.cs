using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpLoader
{
    public static class WinApi
    {
        /// <summary>
        /// Retrieves a string from the specified section in an initialization file.
        /// </summary>
        /// <param name="lpAppName">The name of the section containing the key name.</param>
        /// <param name="lpKeyName">The name of the key whose associated string is to be retrieved.</param>
        /// <param name="lpDefault">A default string.</param>
        /// <param name="lpReturnedString">A pointer to the buffer that receives the retrieved string.</param>
        /// <param name="nSize">The size of the buffer pointed to by the lpReturnedString parameter, in characters.</param>
        /// <param name="lpFileName">The name of the initialization file.</param>
        [DllImport("kernel32")]
        public static extern void GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, uint nSize, string lpFileName);

        /// <summary>
        /// Copies a string into the specified section of an initialization file.
        /// </summary>
        /// <param name="lpAppName">The name of the section to which the string will be copied.</param>
        /// <param name="lpKeyName">The name of the key to be associated with a string.</param>
        /// <param name="lpString">A null-terminated string to be written to the file.</param>
        /// <param name="lpFileName">The name of the initialization file.</param>
        /// <returns></returns>
        [DllImport("kernel32")]
        public static extern bool WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);
    }
}
