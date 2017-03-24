using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SharpLoader.Yami
{
    public static class WinApi
    {
        [DllImport("kernel32")]
        public static extern int OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32")]
        public static extern bool CloseHandle(int hObject);

        [DllImport("kernel32")]
        public static extern bool ReadProcessMemory(int hProcess, IntPtr lpBaseAddress, byte[] buffer, int size, int lpNumberOfBytesRead);

        [DllImport("kernel32")]
        public static extern bool WriteProcessMemory(int hProcess, IntPtr lpBaseAddress, byte[] buffer, int size, int lpNumberOfBytesWritten);

        [DllImport("user32")]
        public static extern short GetAsyncKeyState(Keys vKey);
    }
}
