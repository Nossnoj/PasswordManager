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
            /*Console.WriteLine("Användning: init <Client Path> <Server Path>");

            string clientPath = args[0];
            string serverPath = args[1];*/
            //Möjlig validering för att kontrollera att filerna faktikst döps till client och server


            //MASTER PASSWORD
            Console.Write("Skriv in ett lösenord: ");
            string masterPassword = Console.ReadLine();
            byte[] masterPassWordBytes = Convert.FromBase64String(masterPassword);
         



            //FILVÄGAR
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop); //Kanske ha i projektmappen istället för på Desktop
            string clientFile = Path.Combine(desktopPath, "client.json"); //byt ut client och server mot clientpath och serverpath 
            string serverFile = Path.Combine(desktopPath, "server.json");
            



            //SECRET KEY
            byte[] secretKeyByteArray = GenerateByteArray(16); 

            string secretKey = Convert.ToBase64String(secretKeyByteArray);
            //Console.WriteLine("Your secret key is: " + secretKey);  --> Detta är secret command?

            Dictionary<string, string> secretDict = new Dictionary<string, string>();
            secretDict["secret"] = secretKey;
            string dictJson = JsonSerializer.Serialize(secretDict);

            File.WriteAllText(clientFile, dictJson); // HÄR SPARAS SECRET KEY INUTI CLIENT FILE
            // Your secret key will be printed in plain-text to standard out" - spotta ut secret key i konsol?



            //SERVER
            Dictionary<string, string> server = new Dictionary<string, string>();

            
            Console.WriteLine("STOP");


            byte[] vaultKey = makeVaultKey(masterPassWordBytes, secretKeyByteArray);

            Console.WriteLine(vaultKey);

            Dictionary<string, string> vault = new Dictionary<string, string>();
            vault = Set(vault, "GOOGLE", "password");
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
            byte[] salt = GenerateByteArray(8); //ny salt varje gång

            byte[] concatMasterSecret = mstrpwd.Concat(scrtkey).ToArray();

            Rfc2898DeriveBytes vk = new Rfc2898DeriveBytes(concatMasterSecret, salt, 1000, HashAlgorithmName.SHA256); //SHA256 eller annan variant?
            {
                return vk.GetBytes(32);
            }
        }

        /*static string Get()
        {
            //gagtrhrh
        }*/


        static Dictionary<string, string> Set(Dictionary<string, string> vault, string applikation, string pswrd)
        {
            vault[applikation] = pswrd;
            return vault;
        }


        /* 
        Lista med frågetecken:
        * makeVaultKey metoden - måste vi encrypta den kombinerade master password och secret key eller räcker det med hashning?
        * SHA256, spelar det någon roll vilken SHA man väljer? SHA1, SHA512 etc?
        * Kan secret key vara saltet?
        * Lagrar vi encrypted vault och IV korrekt i Server filen?
        * Kan IV och encryptedVault i AesClass klassen vara public?
        * Läs data från användaren, ska vi tillåta användare att skriva in sin filsöksväg?
        * Vad ska vi göra med saltet så att den går att använda flera gånger, var kan vi lagra den?
         
         
         
         
         
         
         
         
         */
    }
}
