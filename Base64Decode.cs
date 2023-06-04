using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace cryptic
{
    public static class Base64Decode
    {
        public static async Task DecodeFromUrl(string url, string outputFilePath)
        {
            using (var client = new HttpClient())
            {
                var base64EncodedData = await client.GetStringAsync(url);
                var decodedBytes = Convert.FromBase64String(base64EncodedData);
                var decodedText = Encoding.UTF8.GetString(decodedBytes);
                await File.WriteAllTextAsync(outputFilePath, decodedText);
            }
        }
    }
}
