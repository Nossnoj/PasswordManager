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


            //Fokusera på INIT-commandot vilket är kryptera och dekryptera 
            // AES klass länkad i filerna

            //Vault ska vara ett dictionary
            //Server ska vara ett dictionary



            Dictionary<string, string> server = new Dictionary<string, string>();

            //skapar serverFile på desktop med IV
            byte[] IV = GenerateByteArray(16);
            string IVJson = JsonSerializer.Serialize(IV);

            File.WriteAllText(serverFile, "encryptedVault" + IVJson); //byteArray lagras encodad i base64string i serverfilen
            
            Console.WriteLine("STOP");



            byte[] vaultKey = makeVaultKey(masterPassword, secretKey); //lagra vault key som sträng men använd som byte array vid kryptering

            Console.WriteLine(vaultKey);

            Dictionary<string, string> vault = new Dictionary<string, string>();
            vault = addToVault(vault, "GOOGLE", "password");
            /*foreach(var kvp in vault)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value}");
            }*/

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

        static byte[] makeVaultKey(string mstrpwd, string scrtkey)
        {
            byte[] salt = GenerateByteArray(8);

            string concatMasterSecret = $"{mstrpwd}:{scrtkey}";

            Rfc2898DeriveBytes k1 = new Rfc2898DeriveBytes(concatMasterSecret, salt, 1000);
            Rfc2898DeriveBytes k2 = new Rfc2898DeriveBytes(concatMasterSecret, salt);
            Aes encAlg = Aes.Create();
            encAlg.Key = k1.GetBytes(16);

            MemoryStream encryptionStream = new MemoryStream();

            CryptoStream encrypt = new CryptoStream(encryptionStream, encAlg.CreateEncryptor(), CryptoStreamMode.Write);

            byte[] utfD1 = new System.Text.UTF8Encoding(false).GetBytes(concatMasterSecret);

            encrypt.Write(utfD1, 0, utfD1.Length);
            encrypt.FlushFinalBlock();
            encrypt.Close();
            byte[] vaultKey = encryptionStream.ToArray(); //HÄR BLIR DET 48 BYTES
            k1.Reset();
            
            return vaultKey;


            // DECRYPT
            /*Aes decAlg = Aes.Create();
            decAlg.Key = k2.GetBytes(16);
            decAlg.IV = encAlg.IV;
            MemoryStream decryptionStreamBacking = new MemoryStream();
            CryptoStream decrypt = new CryptoStream(decryptionStreamBacking, decAlg.CreateDecryptor(), CryptoStreamMode.Write);
            decrypt.Write(edata1, 0, edata1.Length);
            decrypt.Flush();
            decrypt.Close();
            k2.Reset();
            string data2 = new UTF8Encoding(false).GetString(decryptionStreamBacking.ToArray());
            Console.WriteLine(data2);*/
        }


        static Dictionary<string, string> addToVault(Dictionary<string, string> vault, string applikation, string pswrd)
        {
            vault[applikation] = pswrd;
            return vault;
        }
    }
}
