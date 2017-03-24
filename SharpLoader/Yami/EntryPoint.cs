using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace SharpLoader.Yami
{
    public static class EntryPoint
    {
        public static void Start()
        {
            <swap>
            Console.Write("Looking for CS:GO...");
            Process gameProcess;
            var tries = 0;
            var clientBase = IntPtr.Zero;
            <trash>
            <swap/>

            // Wait for cs:go process
            while (true)
            {
                <swap>
                var processes = Process.GetProcessesByName("csgo");
                <trash>
                <swap/>

                if (processes.Length > 0)
                {
                    <swap>
                    gameProcess = processes[0];
                    <trash>
                    <swap/>

                    break;
                }

                Console.CursorLeft = 0;
                <trash>
                Console.Write("Looking for CS:GO" + new string('.', ++tries % 4) + new string(' ', 3 - tries % 4));
                Thread.Sleep(800);
            }

            Console.CursorLeft = 0;
            Console.WriteLine("Looking for CS:GO...");
            <trash>
            Console.WriteLine(string.Format("Process found (ID: {0})", gameProcess.Id));

            // Scan modules
            Console.WriteLine("Scanning modules...");
            foreach (ProcessModule module in gameProcess.Modules)
            {
                <trash>
                if (module.ModuleName == "client.dll")
                {
                    <trash>
                    clientBase = module.BaseAddress;
                    <trash>
                    Console.WriteLine(string.Format("client.dll found (0x{0})", clientBase.ToString("X")));
                    break;
                }
            }

            <swap>
            var mem = new Memory(gameProcess);
            <trash>
            <swap/>
            Console.WriteLine(string.Format("Handle opened (ID: {0})", mem.Handle));

            Console.WriteLine();
            Console.WriteLine("-=-=- Yami -=-=-");

            <swap>
            var exitKey = Keys.F12;
            var bunnyhopKey = Keys.Space;
            <trash>
            <swap/>

            // Print configuration
            Console.WriteLine("Exit Key: " + exitKey);
            <trash>
            Console.WriteLine("Bunnyhop Key: " + bunnyhopKey);
            <trash>

            while (!IsKeyPressed(exitKey) && !gameProcess.HasExited)
            {
                <swap>
                var localPlayer = new IntPtr(mem.ReadInt(clientBase + Signatures.dwLocalPlayer));
                <trash>
                <swap/>

                var localHealth = mem.ReadInt(localPlayer + NetVars.m_iHealth);

                // LocalPlayer is dead
                if (localHealth == 0)
                {
                    Thread.Sleep(100);
                    continue;
                }

                // Bunnyhop key is not pressed
                if (!IsKeyPressed(bunnyhopKey))
                {
                    Thread.Sleep(10);
                    continue;
                }

                <swap>
                <trash>
                var localFlags = mem.ReadByte(localPlayer + NetVars.m_fFlags);
                <swap/>

                // LocalPlayer is in air
                if (localFlags == 0 || localFlags == 6)
                {
                    Thread.Sleep(1);
                    continue;
                }

                <swap>
                <trash>
                mem.Write(clientBase + Signatures.dwForceJump, 5);
                <swap/>

                Thread.Sleep(20);

                <swap>
                <trash>
                mem.Write(clientBase + Signatures.dwForceJump, 4);
                <swap/>

                Thread.Sleep(100);
            }
        }

        public static bool IsKeyPressed(Keys key)
        {
            var keyState = WinApi.GetAsyncKeyState(key);
            return (keyState & 0x8000) > 0;
        }
    }
}
