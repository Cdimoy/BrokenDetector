using System;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BrokenDetector
{
    struct GamePointer
    {
        int address;
        int offset;
        public GamePointer(int address, int offset)
        {
            this.address = address;
            this.offset = offset;
        }

        public IntPtr Address()
        {
            return new IntPtr(address);
        }
        public int Offset()
        {
            return offset;
        }
    }

    class TouhouGame
    {

        const uint PROCESS_ALL_ACCESS = (0x000F0000 | 0x00100000 | 0xFFF);
        GamePointer CameraChargePointer = new GamePointer(0x004b5654, 0x00016838); // Base address + Offset

        bool isAttached;
        IntPtr hProcess;

        public IntPtr CameraChargeAddress;
        public TouhouGame()
        {
            hProcess = (IntPtr)0;
            isAttached = false;
        }

        public static bool IsRuning(string processName)
        {
            return Process.GetProcessesByName(processName).Length != 0;
        }

        public void Attach(string processName)
        {
            var process = Process.GetProcessesByName(processName).First();

            hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, process.Id);
            isAttached = true;

            uint nBytesRead = 0;

            byte[] lpBuffer = new byte[4];

            ReadProcessMemory(hProcess, CameraChargePointer.Address(), lpBuffer, 4, ref nBytesRead);

            CameraChargeAddress = new IntPtr(BitConverter.ToInt32(lpBuffer, 0) + CameraChargePointer.Offset());
        }

        public bool IsAttached()
        {
            return isAttached;
        }

        public void Detach()
        {
            if (IsRuning("th165"))
                CloseHandle(hProcess);
            isAttached = false;
        }

        public float ReadFloat(IntPtr address)
        {
            uint nBytesRead = 0;

            byte[] lpBuffer = new byte[4];

            ReadProcessMemory(hProcess, address, lpBuffer, 4, ref nBytesRead);

            return BitConverter.ToSingle(lpBuffer, 0);
        }

        public void WriteFloat(IntPtr address, float value)
        {
            uint lpNumberOfBytesWritten = 0;

            byte[] lpBuffer = BitConverter.GetBytes(value);

            WriteProcessMemory(hProcess, address, lpBuffer, 4, ref lpNumberOfBytesWritten);
        }

        ~TouhouGame() {
            if (isAttached)
                CloseHandle(hProcess);
        }


        // Mapped WinAPI functions

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CloseHandle(IntPtr hObject);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, ref uint lpNumberOfBytesWritten);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, ref uint lpNumberOfBytesRead);
    }
}
