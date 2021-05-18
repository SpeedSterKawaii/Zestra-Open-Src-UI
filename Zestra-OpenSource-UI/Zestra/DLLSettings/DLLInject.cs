using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Zestra.DLLSettings
{
    class DLLInject
    {
        [DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true)]
        internal static extern IntPtr LoadLibraryA(string lpFileName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        internal static extern UIntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll")]
        internal static extern IntPtr OpenProcess(ProcessAccess dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        internal static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, UIntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, out IntPtr lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        public bool InitZestraDLL(string dllname)
        {
            if (Process.GetProcessesByName("RobloxPlayerBeta").Length == 0)
            {
                return false;
            }
            Process process = Process.GetProcessesByName("RobloxPlayerBeta")[0];
            byte[] bytes = new ASCIIEncoding().GetBytes(AppDomain.CurrentDomain.BaseDirectory + dllname);
            IntPtr hModule = LoadLibraryA("kernel32.dll");
            UIntPtr procAddress = GetProcAddress(hModule, "LoadLibraryA");
            FreeLibrary(hModule);
            if (procAddress == UIntPtr.Zero)
            {
                return false;
            }
            IntPtr intPtr = OpenProcess(ProcessAccess.AllAccess, false, process.Id);
            if (intPtr == IntPtr.Zero)
            {
                return false;
            }
            IntPtr intPtr2 = VirtualAllocEx(intPtr, (IntPtr)0, (uint)bytes.Length, 12288u, 4u);
            UIntPtr uintPtr;
            IntPtr intPtr3;
            return !(intPtr2 == IntPtr.Zero) && WriteProcessMemory(intPtr, intPtr2, bytes, (uint)bytes.Length, out uintPtr) && !(CreateRemoteThread(intPtr, (IntPtr)0, 0u, procAddress, intPtr2, 0u, out intPtr3) == IntPtr.Zero);
        }

        [Flags]
        public enum ProcessAccess
        {
            AllAccess = 1050235,
            CreateThread = 2,
            DuplicateHandle = 64,
            QueryInformation = 1024,
            SetInformation = 512,
            Terminate = 1,
            VMOperation = 8,
            VMRead = 16,
            VMWrite = 32,
            Synchronize = 1048576
        }
    }
}
