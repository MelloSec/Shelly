using System.IO;
using System.Text;

namespace cryptic
{
    public static class XOR
    {
        public static byte[] XorEncDec(byte[] inputData, string keyword)
        {
            byte[] bufferBytes = new byte[inputData.Length];
            for (int i = 0; i < inputData.Length; i++)
            {
                bufferBytes[i] = (byte)(inputData[i] ^ Encoding.UTF8.GetBytes(keyword)[i % Encoding.UTF8.GetBytes(keyword).Length]);
            }
            return bufferBytes;
        }

        public static void EncryptXORFile(string inputFile, string keyword, string outputFile)
        {
            byte[] inputData = File.ReadAllBytes(inputFile);
            byte[] xorEncrypted = XorEncDec(inputData, keyword);
            File.WriteAllBytes(outputFile, xorEncrypted);
        }

        public static void DecryptXORFile(string inputFile, string keyword, string outputFile)
        {
            byte[] inputData = File.ReadAllBytes(inputFile);
            byte[] xorDecrypted = XorEncDec(inputData, keyword);
            File.WriteAllBytes(outputFile, xorDecrypted);
        }
    }
}
