using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;

namespace PasswordManager
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //string inputPath = Console.ReadLine();

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string clientFile = Path.Combine(desktopPath, "client.json");
            string serverFile = Path.Combine(desktopPath, "server.json");




            //skapar clientFile på desktop med secret key
            byte[] secretKeyByteArray = GenerateByteArray(32); 

            string secretKey = Convert.ToBase64String(secretKeyByteArray);

            Dictionary<string, string> secretDict = new Dictionary<string, string>();
            secretDict["secret"] = secretKey;
            string dictJson = JsonSerializer.Serialize(secretDict);

            File.WriteAllText(clientFile, dictJson);






            //skapar serverFile på desktop med IV
            byte[] IV = GenerateByteArray(16);
            string IVJson = JsonSerializer.Serialize(IV);

            File.WriteAllText(serverFile, "encryptedVault" + IVJson); //byteArray lagras encodad i base64string i serverfilen
            Console.WriteLine("STOP");
        }


        static byte[] GenerateByteArray(int size)
        {
            byte[] byteArray = new byte[size];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(byteArray);
            }

            return byteArray;

            //OVAN TAGET FRÅN https://learn.microsoft.com/mt-mt/dotnet/api/system.security.cryptography.randomnumbergenerator.getbytes?view=netcore-2.2
        }
    }
}
