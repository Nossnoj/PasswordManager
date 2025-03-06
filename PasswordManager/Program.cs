using Microsoft.VisualBasic;
using System.Dynamic;
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
            Console.ForegroundColor = ConsoleColor.Green;
            //attemptDecryptValve("password", "client.json");
            /*Console.WriteLine("Användning: init <Client Path> <Server Path>");

            string clientPath = args[0];
            string serverPath = args[1];*/
            //Möjlig validering för att kontrollera att filerna faktikst döps till client och server

            //MASTER PASSWORD
            Console.Write("Skriv in ett lösenord: ");
            string masterPassword = Console.ReadLine();
            Console.WriteLine("Skriv client.json");
            string clientPath = Console.ReadLine();
            Console.WriteLine("Skriv server.json");
            string serverPath = Console.ReadLine();


            Console.WriteLine("Använd följande kommandon:");
            Console.WriteLine("    init             <client> <server> {<pwd>}                             - Skapa en ny klient- och serverfil");
            Console.WriteLine("    create           <client> <server> {<pwd>} {<secret>}                  - Skapa en ny klient för en existerande server");
            Console.WriteLine("    get              <client> <server> [<prop>] {<pwd>}                    - Hämta ut lösenord");
            Console.WriteLine("    set              <client> <server> <prop> [-g] {<pwd>} {<value>}       - Spara ett lösenord");
            Console.WriteLine("    delete           <client> <server> <prop> {<pwd>}                      - Ta bort ett lösenord");
            Console.WriteLine("    secret           <client>                                              - Visa Secret Key");
            Console.WriteLine("    change           <client> <server> {<pwd>} {<new_pwd>}                 - Ändra huvudlösenord");


            string command = Console.ReadLine();
            switch (command)
            {
                case "init":
                    //init();
                    break;
                case "create":
                    //Create();
                    break;
                case "get":
                    //Get();
                    break;
                case "set":
                    //Set();
                    break;
                case "delete":
                    //Delete();
                    break;
                case "secret":
                    //Secret();
                    break;
                case "change":
                    //Change();
                    break;
            }


            //FILVÄGAR
            init(clientPath, serverPath, masterPassword);

            attemptDecryptValve("password", "client.json");

            Console.WriteLine("STOP");




        }

        static void init(string clientPath, string serverPath, string mstrpwd)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop); //Kanske ha i projektmappen istället för på Desktop


            //CLIENT & SECRET KEY
            string clientFile = Path.Combine(desktopPath, clientPath);
            byte[] secretKeyByteArray = GenerateByteArray(16);
            string secretKey = Convert.ToBase64String(secretKeyByteArray);
            Dictionary<string, string> secretDict = new Dictionary<string, string>();
            secretDict["secret"] = secretKey;
            string dictJson = JsonSerializer.Serialize(secretDict);
            File.WriteAllText(clientFile, dictJson); // HÄR SPARAS SECRET KEY INUTI CLIENT FILE
                                                     // Your secret key will be printed in plain-text to standard out" - spotta ut secret key i konsol?

            //SERVER
            Dictionary<string, string> server = new Dictionary<string, string>();
            string serverFile = Path.Combine(desktopPath, serverPath);


            //VAULT KEY
            byte[] vaultKey = makeVaultKey(mstrpwd, secretKeyByteArray);



            //VAULT
            Dictionary<string, string> vault = new Dictionary<string, string>();
            //vault = Set(vault, "GOOGLE", "password"); 
            string jsonVault = JsonSerializer.Serialize(vault);

            // AES, KRYPTERING OCH LAGRING AV VAULT OCH IV I SERVER FIL
            AesClass aes = new AesClass(jsonVault, vaultKey);
            string IV = Convert.ToBase64String(aes.IV);
            string encryptedVault = Convert.ToBase64String(aes.encryptedVault);
            server["vault"] = encryptedVault;
            server["iv"] = IV;
            string serverJson = JsonSerializer.Serialize(server);
            File.WriteAllText(serverFile, serverJson);

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






        static byte[] makeVaultKey(string mstrpwd, byte[] scrtkey)
        {
            Rfc2898DeriveBytes vk = new Rfc2898DeriveBytes(mstrpwd, scrtkey, 1000, HashAlgorithmName.SHA256); //SHA256 eller annan variant?
            {
                return vk.GetBytes(32);
            }
        }




        static void Create(string path)
        {
            Console.Write("Ange din Secret Key: ");
            string secretKey = Console.ReadLine();
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string newClient = Path.Combine(desktopPath, path);

            Dictionary<string, string> secretDict = new Dictionary<string, string>();
            secretDict["secret"] = secretKey;

        }

        static void attemptDecryptValve(string mstrpwd, string clientPath)// kollar så att Value tillsammans med masterpassword dekryptar valvet. Används för alla metoder som kräver masterpassword
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop); //Kanske ha i projektmappen istället för på Desktop
            string clientFile = Path.Combine(desktopPath, clientPath);
            string s = File.ReadAllText(clientFile);
            Dictionary<string, string> secretDictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(s);
            string scrtKey = secretDictionary["secret"];
            byte[] scrtKeyByteArray = Convert.FromBase64String(scrtKey);

            byte[] vk = makeVaultKey(mstrpwd, scrtKeyByteArray);
            //kolla så att Value tillsammans med masterpassword dekryptar valvet. Används för alla metoder som kräver masterpassword
            //byte[] vk = makeVaultKey(mstrpwd, scrtkey);
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
         
         
         
         
         
         
         
         
         */
    }
}
