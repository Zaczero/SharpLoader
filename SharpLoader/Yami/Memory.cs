using System;
using System.Diagnostics;
using System.Text;

namespace SharpLoader.Yami
{
    public class Memory
    {
        public readonly int Handle;

        public Memory(Process process)
        {
            <swap>
            Handle = WinApi.OpenProcess(0x8 | 0x10 | 0x20, false, process.Id);
            <trash>
            <swap/>
        }

        ~Memory()
        {
            <swap>
            WinApi.CloseHandle(Handle);
            <trash>
            <swap/>
        }

        public byte ReadByte(IntPtr address)
        {
            <swap>
            var buffer = ReadBytes(address, 1);
            <trash>
            <swap/>
            return buffer[0];
        }

        public bool ReadBool(IntPtr address)
        {
            <swap>
            var buffer = ReadBytes(address, 1);
            <trash>
            <swap/>
            return BitConverter.ToBoolean(buffer, 0);
        }

        public int ReadInt(IntPtr address)
        {
            <swap>
            var buffer = ReadBytes(address, 4);
            <trash>
            <swap/>
            return BitConverter.ToInt32(buffer, 0);
        }

        public float ReadFloat(IntPtr address)
        {
            <swap>
            var buffer = ReadBytes(address, 4);
            <trash>
            <swap/>
            return BitConverter.ToSingle(buffer, 0);
        }

        public byte[] ReadBytes(IntPtr address, int bufferSize)
        {
            <swap>
            var buffer = new byte[bufferSize];
            <trash>
            <swap/>
            <swap>
            WinApi.ReadProcessMemory(Handle, address, buffer, buffer.Length, 0);
            <trash>
            <swap/>
            return buffer;
        }

        public void Write(IntPtr address, byte value)
        {
            <swap>
            Write(address, new[] { value });
            <trash>
            <swap/>
        }

        public void Write(IntPtr address, bool value)
        {
            <swap>
            var buffer = BitConverter.GetBytes(value);
            <trash>
            <swap/>
            Write(address, buffer);
        }

        public void Write(IntPtr address, int value)
        {
            <swap>
            var buffer = BitConverter.GetBytes(value);
            <trash>
            <swap/>
            Write(address, buffer);
        }

        public void Write(IntPtr address, float value)
        {
            <swap>
            var buffer = BitConverter.GetBytes(value);
            <trash>
            <swap/>
            Write(address, buffer);
        }

        public void Write(IntPtr address, byte[] buffer)
        {
            <swap>
            WinApi.WriteProcessMemory(Handle, address, buffer, buffer.Length, 0);
            <trash>
            <swap/>
        }
    }
}
