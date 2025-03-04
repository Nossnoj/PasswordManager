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

            File.WriteAllText(clientFile, dictJson); // HÄR SPARAS SECRET KEY INUTI CLIENT FILE
            // Your secret key will be printed in plain-text to standard out" - spotta ut secret key i konsol?

            Dictionary<string, string> server = new Dictionary<string, string>();

            
            Console.WriteLine("STOP");


            byte[] vaultKey = makeVaultKey(masterPassWordBytes, secretKeyByteArray);

            Console.WriteLine(vaultKey);

            Dictionary<string, string> vault = new Dictionary<string, string>();
            vault = addToVault(vault, "GOOGLE", "password");
            string jsonVault = JsonSerializer.Serialize(vault);

            AesClass aes = new AesClass(jsonVault, vaultKey);// Aes-objekt ska inte sparas, men vi måste spara vår IV för att kunna decrypta senare, Vault Key kan genereras på nytt genom Master Password + Secret Key
            Console.WriteLine("Hej");
            string IV = Convert.ToBase64String(aes.IV);
            string encryptedVault = Convert.ToBase64String(aes.encryptedVault);
            server["vault"] = encryptedVault;
            server["iv"] = IV;

            string serverJson = JsonSerializer.Serialize(server);
            
            
            File.WriteAllText(serverFile, serverJson); // Backe måste kolla på detta

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


        /* 
        Lista med frågetecken:
        * makeVaultKey metoden - måste vi encrypta den kombinerade master password och secret key eller räcker det med hashning?
        * Lagrar vi encrypted vault och IV korrekt i Server filen?
        * 
         
         
         
         
         
         
         
         
         
         
         */
    }
}
