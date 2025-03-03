using Microsoft.VisualBasic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml;

namespace PasswordManager
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //string inputPath = Console.ReadLine();

            Console.Write("Skriv in ett lösenord: ");
            string masterPassword = Console.ReadLine();
            byte[] masterPassWordBytes = Convert.FromBase64String(masterPassword);
         
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string clientFile = Path.Combine(desktopPath, "client.json");
            string serverFile = Path.Combine(desktopPath, "server.json");
            

            //skapar clientFile på desktop med secret key
            byte[] secretKeyByteArray = GenerateByteArray(16); 

            string secretKey = Convert.ToBase64String(secretKeyByteArray);

            Dictionary<string, string> secretDict = new Dictionary<string, string>();
            secretDict["secret"] = secretKey;
            string dictJson = JsonSerializer.Serialize(secretDict);

            File.WriteAllText(clientFile, dictJson);





            Dictionary<string, string> server = new Dictionary<string, string>();

            File.WriteAllText(serverFile, "encryptedVault" + "IV"); //byteArray lagras encodad i base64string i serverfilen
            
            Console.WriteLine("STOP");


            byte[] vaultKey = makeVaultKey(masterPassWordBytes, secretKeyByteArray);

            Console.WriteLine(vaultKey);

            Dictionary<string, string> vault = new Dictionary<string, string>();
            vault = addToVault(vault, "GOOGLE", "password");

            AesClass aes = new AesClass(vaultKey);
            Console.WriteLine("Hej");

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

        static byte[] makeVaultKey(byte[] mstrpwd, byte[] scrtkey)
        {
            byte[] salt = GenerateByteArray(8);

            byte[] concatMasterSecret = mstrpwd.Concat(scrtkey).ToArray();

            Rfc2898DeriveBytes k1 = new Rfc2898DeriveBytes(concatMasterSecret, salt, 1000, HashAlgorithmName.SHA256); //SHA256 eller annan variant?
            {
                return k1.GetBytes(32);
            }
        }


        static Dictionary<string, string> addToVault(Dictionary<string, string> vault, string applikation, string pswrd)
        {
            vault[applikation] = pswrd;
            return vault;
        }
    }
}
